using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Robotics.StateMachines;
using Robotics.Controls;
using ActionPlanner.ComplexActions;
using ActionPlanner.Tests.ConfigurationFiles;

namespace ActionPlanner.Tests.StateMachines
{
    public class GPSR
    {

        #region Enums
        /// <summary>
        ///  States of the state machine
        /// </summary>
        private enum States
        {
            /// <summary>
            ///  Configuration state (variables setup)
            /// </summary>
            InitialState,

            EnterArena,
            WaitForQuestion,
            PerformAction,
            ReturnToStartPoint,
            LeaveArena,
            /// <summary>
            ///  Final state of this SM
            /// </summary>
            FinalState            
        }

        /// <summary>
        /// Indicates the STATUS of this SM.
        /// </summary>
        public enum Status
        {
            // TODO: Add the status you need here

            /// <summary>
            /// This SM is ready for execution
            /// </summary>
            Ready,
            /// <summary>
            /// This SM is still running
            /// </summary>
            StillRunning,
            /// <summary>
            /// The execution of this SM was successful.
            /// </summary>
            OK,
            /// <summary>
            /// The execution of this SM was NOT successful.
            /// </summary>
            Failed
        }
        #endregion

        #region Variables
        /// <summary>
        /// Stores the HAL9000Brain instance
        /// </summary>
        private HAL9000Brain brain;
        /// <summary>
        /// Stores the HAL9000CmdMan instance
        /// </summary>
        private HAL9000CmdMan cmdMan;
        /// <summary>
        /// The state machine that executes the test
        /// </summary>
        private FunctionBasedStateMachine SM;
        /// <summary>
        /// Stores the state where the state machine stops
        /// </summary>
        private Status finalStatus;
        /// <summary>
        /// Stores all the configuration variables for this state machine
        /// </summary>
        private GPSR_WORLD SMConfiguration;
        private string recognizedSentence;
        private bool second_confirmation;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a state machine for the test.
        /// </summary>
        /// <param name="brain">HAL9000Brain instance</param>
        /// <param name="cmdMan">HAL9000CmdMan instance</param>
        public GPSR(HAL9000Brain brain, HAL9000CmdMan cmdMan)
        {
            this.brain = brain;
            this.cmdMan = cmdMan;

            finalStatus = Status.Ready;
            second_confirmation = false;

            SM = new FunctionBasedStateMachine();
            SM.AddState(new FunctionState((int)States.InitialState, InitialState));

            //TODO: Link States-enum whith States-Methods here

            SM.AddState(new FunctionState((int)States.EnterArena, EnterArena));
            SM.AddState(new FunctionState((int)States.WaitForQuestion, WaitForQuestion));
            SM.AddState(new FunctionState((int)States.PerformAction, PerformAction));
            SM.AddState(new FunctionState((int)States.LeaveArena, LeaveArena));            
            SM.AddState(new FunctionState((int)States.FinalState, FinalState, true));
            SM.SetFinalState((int)States.FinalState);
        }
        #endregion

        #region Class Methods
        /// <summary>
        /// Executes the state machine
        /// </summary>
        /// <returns>The obtained STATUS when the state machine stops</returns>
        public Status Execute()
        {
            while (this.brain.Status.IsRunning && this.brain.Status.IsExecutingPredefinedTask && !SM.Finished)
            {
                if (this.brain.Status.IsPaused)
                {
                    Thread.Sleep((int)this.brain.Status.BrainWaveType);
                    continue;
                }
                SM.RunNextStep();
            }
            TextBoxStreamWriter.DefaultLog.WriteLine("HAL9000.-> GPSR SM execution finished.");
            return this.finalStatus;
        }
        #endregion

        #region States Methods
        /// <summary>
        /// Configuration state
        /// </summary>
        private int InitialState(int currentState, object o)
        {
            TextBoxStreamWriter.DefaultLog.WriteLine("HAL9000.-> Initializing GPSR test.");

            //Load the WORLD configuration for this test
            SMConfiguration = new GPSR_WORLD();

            // TODO: Change the next status
            return (int)States.EnterArena;
        }
        private int EnterArena(int currentState, object o)
        {
            TextBoxStreamWriter.DefaultLog.WriteLine("HAL9000.-> State reached EnterArena");

            SM_EnterArena enterArena = new SM_EnterArena(brain, cmdMan, "entrancelocation");
            SM_EnterArena.FinalStates enterArena_status = enterArena.Execute();

            if (enterArena_status == SM_EnterArena.FinalStates.OK)
            {
                this.cmdMan.SPG_GEN_say("Please tell me what you would like me to do.");
                brain.recognizedSentences.Clear();
                return (int)States.WaitForQuestion;
            }

            return currentState;
        }

        // TODO: Add all the State-Mehtods you need here
        private int WaitForQuestion(int currentState, object o)
        {
            //ver si reconocio algo
            if(brain.recognizedSentences.Count>0)
            {
                //reconocio
                recognizedSentence = brain.recognizedSentences.Dequeue();

                this.cmdMan.SPG_GEN_say("Did you say: " + recognizedSentence + "?");

                if (this.brain.WaitForUserConfirmation())
                    return (int)States.PerformAction;

                if (!second_confirmation)
                {
                    second_confirmation = true;
                    return currentState;
                }

                brain.recognizedSentences.Clear();
                this.cmdMan.SPG_GEN_say("Please tell me again what you would like me to do.");
            }

            second_confirmation = false;
            Thread.Sleep(3000);
            return currentState;
        }

        private int PerformAction(int currentState, object o)
        {
            switch (recognizedSentence)
            {
                case "get the coke from the shelf and bring it to me":
                    //ejecutar la sm correspondiente
                    GetCoke sm0 = new GetCoke(this.brain, this.cmdMan);
                    sm0.Execute();
                    break;
                case "grasp the cereal from the kitchen table and detect a person":
                    GraspCereal sm1 = new GraspCereal(this.brain, this.cmdMan);
                    sm1.Execute();
                    break;
                case "take the jam from the side table and deliver it to the shelf":
                    //ejecutar la sm correspondiente
                    TakeJam sm2 = new TakeJam(this.brain, this.cmdMan);
                    sm2.Execute();
                    break;
                case "find a person in the livingroom and answer a question":
                    //ejecutar la sm correspondiente
                    LivingPerson sm3 = new LivingPerson(this.brain, this.cmdMan);
                    sm3.Execute();
                    break;
                case "find a person in the bedroom and say the name of your team":
                    //ejecutar la sm correspondiente
                    BedPerson sm4 = new BedPerson(this.brain, this.cmdMan);
                    sm4.Execute();
                    break;
                case "look for a person in the hall and tell your name":
                    //ejecutar la sm correspondiente
                    HallPerson sm5 = new HallPerson(this.brain, this.cmdMan);
                    sm5.Execute();
                    break;
                case "look for a person in the bedroom and answer a question":
                    //ejecutar la sm correspondiente
                    BedPersonQuestion sm6 = new BedPersonQuestion(this.brain, this.cmdMan);
                    sm6.Execute();
                    break;
            }
            return (int)States.LeaveArena;
        }

        private int LeaveArena(int currentState, object o)
        {

            if(!cmdMan.MVN_PLN_getclose("exit", 20000))
                if (!cmdMan.MVN_PLN_getclose("exit", 20000))
                    cmdMan.MVN_PLN_getclose("exit", 20000);

            return (int)States.FinalState;
        }

        /// <summary>
        /// Final state of this SM
        /// </summary>
        private int FinalState(int currentState, object o)
        {
            return currentState;
        }
        #endregion
    }
}
