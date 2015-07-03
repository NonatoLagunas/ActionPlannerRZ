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
    public class GraspCereal
    {

        #region Enums
        /// <summary>
        ///  States of the "Navigation Test" state machine
        /// </summary>
        private enum States
        {
            /// <summary>
            ///  Configuration state (variables setup)
            /// </summary>
            InitialState,

            //TODO: Add the states you need here
            PerformAction,
            FindPerson,
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

        private string[] armsOrder;
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
        private NavigationTest_WORLD SMConfiguration;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a state machine for the Navigation test.
        /// </summary>
        /// <param name="brain">HAL9000Brain instance</param>
        /// <param name="cmdMan">HAL9000CmdMan instance</param>
        public GraspCereal(HAL9000Brain brain, HAL9000CmdMan cmdMan)
        {
            this.brain = brain;
            this.cmdMan = cmdMan;

            finalStatus = Status.Ready;

            SM = new FunctionBasedStateMachine();
            SM.AddState(new FunctionState((int)States.InitialState, InitialState));

            //TODO: Link States-enum whith States-Methods here            
            SM.AddState(new FunctionState((int)States.PerformAction, PerformAction));
            SM.AddState(new FunctionState((int)States.FindPerson, FindPerson));

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
            TextBoxStreamWriter.DefaultLog.WriteLine("HAL9000.-> Navigation Test SM execution finished.");
            return this.finalStatus;
        }
        #endregion

        #region States Methods
        /// <summary>
        /// Configuration state
        /// </summary>
        private int InitialState(int currentState, object o)
        {
            TextBoxStreamWriter.DefaultLog.WriteLine("HAL9000.-> Initializing test.");

            // TODO: Change the next status
            return (int)States.PerformAction;
        }

        // TODO: Add all the State-Mehtods you need here
        private int PerformAction(int currentState, object o)
        {
            //"grasp the cereal from the kitchen table and detect a person"
            //go to kitchen

            brain.SayAsync("I am going to the kitchen table.");

            if (!cmdMan.HEAD_lookat(0, -1, 10000))
                if (!cmdMan.HEAD_lookat(0, -1, 10000))
                    cmdMan.HEAD_lookat(0, -1, 10000);
            if (!cmdMan.ARMS_goto("standby", 10000))
                if (!cmdMan.ARMS_goto("standby", 10000))
                    cmdMan.ARMS_goto("standby", 10000);
            if (!cmdMan.MVN_PLN_getclose("kitchentable", 10000))
                if (!cmdMan.MVN_PLN_getclose("kitchentable", 10000))
                    cmdMan.MVN_PLN_getclose("kitchentable", 10000);

            //find and take the cereal (try 3 times)
            if (!cmdMan.ARMS_goto("home", 10000))
                if (!cmdMan.ARMS_goto("home", 10000))
                    cmdMan.ARMS_goto("home", 10000);

            //align to table
            //find the cereal
            int attemps = 0;
            SM_SearchAndTakeObject.FinalStates state;
            state = SM_SearchAndTakeObject.FinalStates.StillRunning;
            while (attemps < 5 && state != SM_SearchAndTakeObject.FinalStates.OK)
            {
                SM_SearchAndTakeObject sm = new SM_SearchAndTakeObject(this.brain, this.cmdMan, false, new string[] { "cereal" }, 2);
                state = sm.Execute();
                if (state == SM_SearchAndTakeObject.FinalStates.OK)
                    this.armsOrder = sm.ArmsOrder;

                attemps++;
            }

            //go to the bedroom
            if (!cmdMan.ARMS_goto("standby", 10000))
                if (!cmdMan.ARMS_goto("standby", 10000))
                    cmdMan.ARMS_goto("standby", 10000);
            if (!cmdMan.HEAD_lookat(0, -1, 10000))
                if (!cmdMan.HEAD_lookat(0, -1, 10000))
                    cmdMan.HEAD_lookat(0, -1, 10000);
            if (!cmdMan.MVN_PLN_getclose("kitchen", 10000))
                if (!cmdMan.MVN_PLN_getclose("kitchen", 10000))
                    cmdMan.MVN_PLN_getclose("kitchen", 10000);
            if (!cmdMan.HEAD_lookat(0, 0, 10000))
                if (!cmdMan.HEAD_lookat(0, 0, 10000))
                    cmdMan.HEAD_lookat(0, 0, 10000);

            if (!cmdMan.ARMS_goto("home", 10000))
                if (!cmdMan.ARMS_goto("home", 10000))
                    cmdMan.ARMS_goto("home", 10000);

            return (int)States.FindPerson;
        }

        private int FindPerson(int currentState, object o)
        {
            //buscar el rostro en el centro del cuarto girando a 3 angulos distintos 
            int searchTimes = 0;
            bool humanFound = false;
            string hname;
            while (!humanFound && searchTimes < 3)
            {
                //search in a diferent position each time
                cmdMan.MVN_PLN_move(0.0, (searchTimes == 0) ? 0 : ((searchTimes == 1) ? 20 : -40), 60000);


                if (cmdMan.ST_PLN_findhuman("human", "", 10000, out hname))
                    humanFound = true;

                searchTimes++;
            }
            if (humanFound)
                brain.SayAsync("Hello human! I found you!");
            else
                brain.SayAsync("I could not find the person.");
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
