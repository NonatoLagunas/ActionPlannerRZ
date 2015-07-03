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
    public class GetCoke
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
        private NavigationTest_WORLD SMConfiguration;
        private string[] armsOrder;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a state machine for the Navigation test.
        /// </summary>
        /// <param name="brain">HAL9000Brain instance</param>
        /// <param name="cmdMan">HAL9000CmdMan instance</param>
        public GetCoke(HAL9000Brain brain, HAL9000CmdMan cmdMan)
        {
            this.brain = brain;
            this.cmdMan = cmdMan;

            finalStatus = Status.Ready;
            SM = new FunctionBasedStateMachine();
            SM.AddState(new FunctionState((int)States.InitialState, InitialState));

            SM.AddState(new FunctionState((int)States.PerformAction, PerformAction));
            //TODO: Link States-enum whith States-Methods here

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
            //get the coke from the shelf and bring it to me

            //go to shelf 
            if(!cmdMan.HEAD_lookat(0, -1, 10000))
                if (!cmdMan.HEAD_lookat(0, -1, 10000))
                    cmdMan.HEAD_lookat(0, -1, 10000);

            brain.SayAsync("I am going to the shelf.");

            if (!cmdMan.ARMS_goto("standby", 10000))
                if (!cmdMan.ARMS_goto("standby", 10000))
                    cmdMan.ARMS_goto("standby", 10000);

            if(!cmdMan.MVN_PLN_getclose("shelf", 10000))
                if(!cmdMan.MVN_PLN_getclose("shelf", 10000))
                    cmdMan.MVN_PLN_getclose("shelf", 10000);

            //find and take the coke (try 3 times)
            if (!cmdMan.ARMS_goto("home", 10000))
                if (!cmdMan.ARMS_goto("home", 10000))
                    cmdMan.ARMS_goto("home", 10000);
            int attemps=0;
            SM_SearchAndTakeObject.FinalStates state;
            state = SM_SearchAndTakeObject.FinalStates.StillRunning;
            while (attemps < 5 && state != SM_SearchAndTakeObject.FinalStates.OK)
            {
                SM_SearchAndTakeObject sm = new SM_SearchAndTakeObject(this.brain, this.cmdMan, false, new string[] { "coke" }, 2);
                state = sm.Execute();
                if (state == SM_SearchAndTakeObject.FinalStates.OK)
                    this.armsOrder = sm.ArmsOrder;

                attemps++;
            }

            //go to the bring location
            if (!cmdMan.ARMS_goto("navigation", 10000))
                if (!cmdMan.ARMS_goto("navigation", 10000))
                    cmdMan.ARMS_goto("navigation", 10000);
            if (!cmdMan.HEAD_lookat(0, -1, 10000))
                if (!cmdMan.HEAD_lookat(0, -1, 10000))
                    cmdMan.HEAD_lookat(0, -1, 10000);
            if (!cmdMan.MVN_PLN_getclose("entrancelocation", 10000))
                if (!cmdMan.MVN_PLN_getclose("entrancelocation", 10000))
                    cmdMan.MVN_PLN_getclose("entrancelocation", 10000);
            if (!cmdMan.HEAD_lookat(0, 0, 10000))
                if (!cmdMan.HEAD_lookat(0, 0, 10000))
                    cmdMan.HEAD_lookat(0, 0, 10000);

            brain.SayAsync("human, please take the coke");
            //bring the coke
            cmdMan.ST_PLN_drop(armsOrder[0], 30000);

            //if(!cmdMan.ARMS_la_goto("deliver",10000))
              //  if (!cmdMan.ARMS_la_goto("deliver", 10000))
                //    cmdMan.ARMS_la_goto("deliver", 10000);
            //cmdMan.ARMS_la_opengrip(1000);
            //cmdMan.ARMS_ra_opengrip(1000);

            //bring the coke
            if (!cmdMan.ARMS_goto("standby", 10000))
                if (!cmdMan.ARMS_goto("standby", 10000))
                    cmdMan.ARMS_goto("standby", 10000);

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
