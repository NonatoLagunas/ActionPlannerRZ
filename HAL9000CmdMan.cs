using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using Robotics.API;
using Robotics.Controls;
using Robotics.Mathematics;

namespace ActionPlanner
{
	public enum JustinaCommands
	{
		BLK_alive, BLK_read_var, BLK_list_vars, BLK_read_vars, BLK_suscribevar, BLK_create_var,

        ARMS_ra_abspos, ARMS_ra_artpos, ARMS_ra_goto, ARMS_ra_move, ARMS_ra_getabspos, ARMS_ra_reachable,
        ARMS_la_abspos, ARMS_la_artpos, ARMS_la_goto, ARMS_la_move, ARMS_la_reachable,
        ARMS_ra_orientation, ARMS_ra_opengrip, ARMS_ra_closegrip, ARMS_ra_getgripstatus, ARMS_ra_torque,
        ARMS_ra_servotorqueon, ARMS_ra_relpos, ARMS_la_opengrip, ARMS_la_closegrip,

		HEAD_lookat, HEAD_show, HEAD_search, HEAD_stop, HEAD_lookatloop, HEAD_lookatrel, HEAD_lookatobject,
        HEAD_followskeleton, HEAD_stopfollowskeleton,

		MVN_PLN_pause, MVN_PLN_move, MVN_PLN_goto, MVN_PLN_addobject, MVN_PLN_obstacle, MVN_PLN_position,
		MVN_PLN_enablelaser, MVN_PLN_disablelaser, MVN_PLN_getclose, MVN_PLN_go_to_room, MVN_PLN_go_to_region,
		MVN_PLN_mv, MVN_PLN_ic, MVN_PLN_stop, MVN_PLN_setspeeds, MVN_PLN_report, MVN_PLN_getpos, MVN_PLN_lectures,
		MVN_PLN_startfollowhuman, MVN_PLN_stopfollowhuman, MVN_PLN_selfterminate,

        OBJ_FNDT_find, OBJ_FNDT_findontable, OBJ_FNDT_removetable, OBJ_FNDT_findedge,

		SENSORS_start, SENSORS_stop,

		SP_GEN_say, SP_GEN_asay, SP_GEN_read, SP_GEN_aread, SP_GEN_aplay, SP_GEN_playloop, SP_GEN_shutup,
		SP_GEN_play, SP_GEN_voice,

		SP_REC_na, SP_REC_status, SP_REC_grammar, SP_REC_words,

		PRS_FND_find, PRS_FND_remember, PRS_FND_auto, PRS_FND_sleep, PRS_FND_shutdown, PRS_FND_amnesia,

		ST_PLN_alignhuman, ST_PLN_cleanplane, ST_PLN_comewith, ST_PLN_dopresentation, ST_PLN_drop, ST_PLN_findhuman, 
		ST_PLN_findhumanobject, ST_PLN_findobject, ST_PLN_findtable, ST_PLN_grab, ST_PLN_greethuman, ST_PLN_release, 
		ST_PLN_rememberhuman, ST_PLN_searchobject, ST_PLN_seehand, ST_PLN_seeobject, ST_PLN_shutdown, ST_PLN_take,
        ST_PLN_deliverobject,

        TORSO_mv, TORSO_abspos, TORSO_relpos
	}

	public enum RegionType { Room, Region, Location }
	
	public class HAL9000CmdMan:CommandManager
	{
		private const int TotalExistingCommands = 150;
		private JustinaCmdAndResp[] justinaCmdAndResp;
		private SortedList<string, JustinaCmdAndResp> sortedCmdAndResp;
		private HAL9000Status status;

		public HAL9000CmdMan(HAL9000Status status)
			: base()
		{
			this.status = status;
			this.ResponseReceived += new ResponseReceivedEventHandler(HAL9000CmdMan_ResponseReceived);
			this.justinaCmdAndResp = new JustinaCmdAndResp[TotalExistingCommands];
			this.sortedCmdAndResp = new SortedList<string, JustinaCmdAndResp>();
			this.LoadJustinaCommands();
		}

		private void HAL9000CmdMan_ResponseReceived(Response response)
		{
			this.ParseResponse(response);
		}

		protected override void ParseResponse(Response response)
		{
			base.ParseResponse(response);

			this.sortedCmdAndResp[response.CommandName].Response = response;
			this.sortedCmdAndResp[response.CommandName].IsResponseReceived = true;
            TextBoxStreamWriter.DefaultLog.WriteLine(2, "HAL9000CmdMan: Response Received: " +
                response.StringToSend);
		}

		private void LoadJustinaCommands()
		{
			#region Commands for BLK
			this.justinaCmdAndResp[(int)JustinaCommands.BLK_alive] = new JustinaCmdAndResp("alive");
            this.justinaCmdAndResp[(int)JustinaCommands.BLK_create_var] = new JustinaCmdAndResp("create_var");
            this.justinaCmdAndResp[(int)JustinaCommands.BLK_list_vars] = new JustinaCmdAndResp("list_vars");
            this.justinaCmdAndResp[(int)JustinaCommands.BLK_read_var] = new JustinaCmdAndResp("read_var");
            this.justinaCmdAndResp[(int)JustinaCommands.BLK_read_vars] = new JustinaCmdAndResp("read_vars");
            this.justinaCmdAndResp[(int)JustinaCommands.BLK_suscribevar] = new JustinaCmdAndResp("subscribe_var");
			#endregion
			#region Commands for ARMS
			this.justinaCmdAndResp[(int)JustinaCommands.ARMS_ra_abspos] = new JustinaCmdAndResp("ra_abspos");
            this.justinaCmdAndResp[(int)JustinaCommands.ARMS_ra_artpos] = new JustinaCmdAndResp("ra_artpos");
            this.justinaCmdAndResp[(int)JustinaCommands.ARMS_ra_goto] = new JustinaCmdAndResp("ra_goto");
            this.justinaCmdAndResp[(int)JustinaCommands.ARMS_ra_move] = new JustinaCmdAndResp("ra_move");
            this.justinaCmdAndResp[(int)JustinaCommands.ARMS_ra_closegrip] = new JustinaCmdAndResp("ra_closegrip");
            this.justinaCmdAndResp[(int)JustinaCommands.ARMS_ra_opengrip] = new JustinaCmdAndResp("ra_opengrip");
            this.justinaCmdAndResp[(int)JustinaCommands.ARMS_ra_reachable] = new JustinaCmdAndResp("ra_reachable");
            this.justinaCmdAndResp[(int)JustinaCommands.ARMS_la_abspos] = new JustinaCmdAndResp("la_abspos");
            this.justinaCmdAndResp[(int)JustinaCommands.ARMS_la_artpos] = new JustinaCmdAndResp("la_artpos");
            this.justinaCmdAndResp[(int)JustinaCommands.ARMS_la_goto] = new JustinaCmdAndResp("la_goto");
            this.justinaCmdAndResp[(int)JustinaCommands.ARMS_la_move] = new JustinaCmdAndResp("la_move");
            this.justinaCmdAndResp[(int)JustinaCommands.ARMS_la_closegrip] = new JustinaCmdAndResp("la_closegrip");
            this.justinaCmdAndResp[(int)JustinaCommands.ARMS_la_opengrip] = new JustinaCmdAndResp("la_opengrip");
            this.justinaCmdAndResp[(int)JustinaCommands.ARMS_la_reachable] = new JustinaCmdAndResp("la_reachable");
			#endregion
			#region Commands for HEAD
            this.justinaCmdAndResp[(int)JustinaCommands.HEAD_followskeleton] = new JustinaCmdAndResp("hd_followskeleton");
            this.justinaCmdAndResp[(int)JustinaCommands.HEAD_stopfollowskeleton] = new JustinaCmdAndResp("hd_stopfollowskeleton");
			this.justinaCmdAndResp[(int)JustinaCommands.HEAD_lookat] = new JustinaCmdAndResp("hd_lookat");
			this.justinaCmdAndResp[(int)JustinaCommands.HEAD_search] = new JustinaCmdAndResp("hd_search");
			this.justinaCmdAndResp[(int)JustinaCommands.HEAD_show] = new JustinaCmdAndResp("hd_show");
			#endregion
			#region Commands for MVN-PLN
			this.justinaCmdAndResp[(int)JustinaCommands.MVN_PLN_getclose] = new JustinaCmdAndResp("mp_getclose");
			this.justinaCmdAndResp[(int)JustinaCommands.MVN_PLN_goto] = new JustinaCmdAndResp("mp_goto");
			this.justinaCmdAndResp[(int)JustinaCommands.MVN_PLN_move] = new JustinaCmdAndResp("mp_move");
            this.justinaCmdAndResp[(int)JustinaCommands.MVN_PLN_obstacle] = new JustinaCmdAndResp("mp_obstacle");
			this.justinaCmdAndResp[(int)JustinaCommands.MVN_PLN_position] = new JustinaCmdAndResp("mp_position");
			this.justinaCmdAndResp[(int)JustinaCommands.MVN_PLN_report] = new JustinaCmdAndResp("mp_report");
            this.justinaCmdAndResp[(int)JustinaCommands.MVN_PLN_setspeeds] = new JustinaCmdAndResp("mp_setspeeds");
			this.justinaCmdAndResp[(int)JustinaCommands.MVN_PLN_startfollowhuman] = new JustinaCmdAndResp("mp_startfollowhuman");
			this.justinaCmdAndResp[(int)JustinaCommands.MVN_PLN_stopfollowhuman] = new JustinaCmdAndResp("mp_stopfollowhuman");
			#endregion
            #region Commands for OBJ_FNDT
            this.justinaCmdAndResp[(int)JustinaCommands.OBJ_FNDT_find] = new JustinaCmdAndResp("oft_find");
            this.justinaCmdAndResp[(int)JustinaCommands.OBJ_FNDT_findontable] = new JustinaCmdAndResp("oft_findontable");
            this.justinaCmdAndResp[(int)JustinaCommands.OBJ_FNDT_findedge] = new JustinaCmdAndResp("oft_findedge");
            #endregion
            #region Commands for PRS-FND
            this.justinaCmdAndResp[(int)JustinaCommands.PRS_FND_sleep] = new JustinaCmdAndResp("pf_sleep");
            this.justinaCmdAndResp[(int)JustinaCommands.PRS_FND_amnesia] = new JustinaCmdAndResp("pf_amnesia");
			#endregion
			#region Commands for SPG-GEN
			//this.justinaCmdAndResp[(int)JustinaCommands.SP_GEN_aread] = new JustinaCmdAndResp("spg_aread");
			this.justinaCmdAndResp[(int)JustinaCommands.SP_GEN_asay] = new JustinaCmdAndResp("spg_asay");
			//this.justinaCmdAndResp[(int)JustinaCommands.SP_GEN_read] = new JustinaCmdAndResp("spg_read");
			this.justinaCmdAndResp[(int)JustinaCommands.SP_GEN_say] = new JustinaCmdAndResp("spg_say");
            this.justinaCmdAndResp[(int)JustinaCommands.SP_GEN_shutup] = new JustinaCmdAndResp("spg_shutup");
			#endregion
            #region Commands for SP-REC
            this.justinaCmdAndResp[(int)JustinaCommands.SP_REC_grammar] = new JustinaCmdAndResp("spr_grammar");
            #endregion
            #region Commands for ST-PLN
            this.justinaCmdAndResp[(int)JustinaCommands.ST_PLN_alignhuman] = new JustinaCmdAndResp("align_human");
			this.justinaCmdAndResp[(int)JustinaCommands.ST_PLN_cleanplane] = new JustinaCmdAndResp("clean_plane");
			this.justinaCmdAndResp[(int)JustinaCommands.ST_PLN_comewith] = new JustinaCmdAndResp("come_with");
			this.justinaCmdAndResp[(int)JustinaCommands.ST_PLN_dopresentation] = new JustinaCmdAndResp("dopresentation");
			this.justinaCmdAndResp[(int)JustinaCommands.ST_PLN_drop] = new JustinaCmdAndResp("drop");
			this.justinaCmdAndResp[(int)JustinaCommands.ST_PLN_findhuman] = new JustinaCmdAndResp("find_human");
			this.justinaCmdAndResp[(int)JustinaCommands.ST_PLN_findhumanobject] = new JustinaCmdAndResp("find_human_object");
			this.justinaCmdAndResp[(int)JustinaCommands.ST_PLN_findobject] = new JustinaCmdAndResp("find_object");
			this.justinaCmdAndResp[(int)JustinaCommands.ST_PLN_findtable] = new JustinaCmdAndResp("find_table");
			this.justinaCmdAndResp[(int)JustinaCommands.ST_PLN_grab] = new JustinaCmdAndResp("grab");
			this.justinaCmdAndResp[(int)JustinaCommands.ST_PLN_greethuman] = new JustinaCmdAndResp("greethuman");
			this.justinaCmdAndResp[(int)JustinaCommands.ST_PLN_release] = new JustinaCmdAndResp("release");
			this.justinaCmdAndResp[(int)JustinaCommands.ST_PLN_rememberhuman] = new JustinaCmdAndResp("remember_human");
			this.justinaCmdAndResp[(int)JustinaCommands.ST_PLN_searchobject] = new JustinaCmdAndResp("search_object");
			this.justinaCmdAndResp[(int)JustinaCommands.ST_PLN_seehand] = new JustinaCmdAndResp("see_hand");
			this.justinaCmdAndResp[(int)JustinaCommands.ST_PLN_seeobject] = new JustinaCmdAndResp("see_object");
			this.justinaCmdAndResp[(int)JustinaCommands.ST_PLN_shutdown] = new JustinaCmdAndResp("shutdown");
			this.justinaCmdAndResp[(int)JustinaCommands.ST_PLN_take] = new JustinaCmdAndResp("take");
            this.justinaCmdAndResp[(int)JustinaCommands.ST_PLN_deliverobject] = new JustinaCmdAndResp("deliverobject");
			#endregion
            #region Commands for TORSO
            this.justinaCmdAndResp[(int)JustinaCommands.TORSO_abspos] = new JustinaCmdAndResp("trs_abspos");
            this.justinaCmdAndResp[(int)JustinaCommands.TORSO_mv] = new JustinaCmdAndResp("trs_mv");
            this.justinaCmdAndResp[(int)JustinaCommands.TORSO_relpos] = new JustinaCmdAndResp("trs_relpos");
            #endregion

            foreach (JustinaCmdAndResp jcar in this.justinaCmdAndResp)
				if (jcar != null)
					this.sortedCmdAndResp.Add(jcar.Command.CommandName, jcar);

			#region Old Code
			/*
			#region Sorted Commands for ARMS
			this.sortedCmdAndResp.Add("ra_goto", this.justinaCmdAndResp[(int)JustinaCommands.ARMS_ra_goto]);
			this.sortedCmdAndResp.Add("ra_move", this.justinaCmdAndResp[(int)JustinaCommands.ARMS_ra_move]);
			#endregion
			#region Sorted Commands for HEAD
			this.sortedCmdAndResp.Add("hd_lookat", this.justinaCmdAndResp[(int)JustinaCommands.HEAD_lookat]);
			this.sortedCmdAndResp.Add("hd_search", this.justinaCmdAndResp[(int)JustinaCommands.HEAD_search]);
			this.sortedCmdAndResp.Add("hd_show", this.justinaCmdAndResp[(int)JustinaCommands.HEAD_show]);
			#endregion
			#region Sorted Commands for MVN-PLN
			this.sortedCmdAndResp.Add("mp_getclose", this.justinaCmdAndResp[(int)JustinaCommands.MVN_PLN_getclose]);
			this.sortedCmdAndResp.Add("mp_goto", this.justinaCmdAndResp[(int)JustinaCommands.MVN_PLN_goto]);
			this.sortedCmdAndResp.Add("mp_move", this.justinaCmdAndResp[(int)JustinaCommands.MVN_PLN_move]);
			this.sortedCmdAndResp.Add("mp_position", this.justinaCmdAndResp[(int)JustinaCommands.MVN_PLN_position]);
			this.sortedCmdAndResp.Add("mp_startfollowhuman", this.justinaCmdAndResp[(int)JustinaCommands.MVN_PLN_startfollowhuman]);
			this.sortedCmdAndResp.Add("mp_stopfollowhuman", this.justinaCmdAndResp[(int)JustinaCommands.MVN_PLN_stopfollowhuman]);
			#endregion
			#region Sorted Commands for SPG-GEN
			//this.sortedCmdAndResp.Add("spg_aread", this.justinaCmdAndResp[(int)JustinaCommands.SP_GEN_aread]);
			//this.sortedCmdAndResp.Add("spg_asay", this.justinaCmdAndResp[(int)JustinaCommands.SP_GEN_asay]);
			//this.sortedCmdAndResp.Add("spg_read", this.justinaCmdAndResp[(int)JustinaCommands.SP_GEN_read]);
			this.sortedCmdAndResp.Add("spg_say", this.justinaCmdAndResp[(int)JustinaCommands.SP_GEN_say]);
			#endregion
			#region Sorted Commands for ST-PLN
			this.sortedCmdAndResp.Add("align_human", this.justinaCmdAndResp[(int)JustinaCommands.ST_PLN_alignhuman]);
			this.sortedCmdAndResp.Add("clean_plane", this.justinaCmdAndResp[(int)JustinaCommands.ST_PLN_cleanplane]);
			this.sortedCmdAndResp.Add("come_with", this.justinaCmdAndResp[(int)JustinaCommands.ST_PLN_comewith]);
			this.sortedCmdAndResp.Add("dopresentation", this.justinaCmdAndResp[(int)JustinaCommands.ST_PLN_dopresentation]);
			this.sortedCmdAndResp.Add("drop", this.justinaCmdAndResp[(int)JustinaCommands.ST_PLN_drop]);
			this.sortedCmdAndResp.Add("find_human", this.justinaCmdAndResp[(int)JustinaCommands.ST_PLN_findhuman]);
			this.sortedCmdAndResp.Add("find_human_object", this.justinaCmdAndResp[(int)JustinaCommands.ST_PLN_findhumanobject]);
			this.sortedCmdAndResp.Add("find_object", this.justinaCmdAndResp[(int)JustinaCommands.ST_PLN_findobject]);
			this.sortedCmdAndResp.Add("find_table", this.justinaCmdAndResp[(int)JustinaCommands.ST_PLN_findtable]);
			this.sortedCmdAndResp.Add("grab", this.justinaCmdAndResp[(int)JustinaCommands.ST_PLN_grab]);
			this.sortedCmdAndResp.Add("greethuman", this.justinaCmdAndResp[(int)JustinaCommands.ST_PLN_greethuman]);
			this.sortedCmdAndResp.Add("release", this.justinaCmdAndResp[(int)JustinaCommands.ST_PLN_release]);
			this.sortedCmdAndResp.Add("remember_human", this.justinaCmdAndResp[(int)JustinaCommands.ST_PLN_rememberhuman]);
			this.sortedCmdAndResp.Add("search_object", this.justinaCmdAndResp[(int)JustinaCommands.ST_PLN_searchobject]);
			this.sortedCmdAndResp.Add("see_hand", this.justinaCmdAndResp[(int)JustinaCommands.ST_PLN_seehand]);
			this.sortedCmdAndResp.Add("see_object", this.justinaCmdAndResp[(int)JustinaCommands.ST_PLN_seeobject]);
			this.sortedCmdAndResp.Add("shutdown", this.justinaCmdAndResp[(int)JustinaCommands.ST_PLN_shutdown]);
			this.sortedCmdAndResp.Add("take", this.justinaCmdAndResp[(int)JustinaCommands.ST_PLN_take]);
			#endregion
			 */
			#endregion
		}

		private void TerminateProcess(string processName, int waitTime_ms)
		{
			Process[] processesToTerminate = Process.GetProcessesByName(processName);
			foreach (Process p in processesToTerminate)
			{
				try
				{
					if (!p.HasExited && p.Responding)
						p.Close();
				}
				catch { }
			}
			Thread.Sleep(waitTime_ms);
			foreach (Process p in processesToTerminate)
			{
				try
				{
					p.Kill();
				}
				catch { }
			}
		}

		private void RunProcess(string path, string args)
		{
			ProcessStartInfo psi = new ProcessStartInfo();
			psi.FileName = path;
			psi.Arguments = args;
			psi.WindowStyle = ProcessWindowStyle.Minimized;
			Process.Start(psi);
		}

		private void SetupAndSendCommand(JustinaCommands justinaCmd, string parameters)
		{
			this.justinaCmdAndResp[(int)justinaCmd].IsResponseReceived = false;
			this.justinaCmdAndResp[(int)justinaCmd].Command.Parameters = parameters;
			this.SendCommand(this.justinaCmdAndResp[(int)justinaCmd].Command);
			TextBoxStreamWriter.DefaultLog.WriteLine(2, "HAL9000CmdMan: Send command: " +
				this.justinaCmdAndResp[(int)justinaCmd].Command.StringToSend);
		}

		public bool WaitForResponse(JustinaCommands expectedCmdResp, int timeOut_ms)
		{
			int sleepsNumber = (int)(((double)timeOut_ms) / ((double)this.status.BrainWaveType));

			while (!this.IsResponseReceived(expectedCmdResp) && this.status.IsRunning &&
				this.status.IsExecutingPredefinedTask && sleepsNumber-- > 0)
				Thread.Sleep((int)this.status.BrainWaveType);

			if (this.IsResponseReceived(expectedCmdResp))
				return this.justinaCmdAndResp[(int)expectedCmdResp].Response.Success;
			else return false;
		}

		/// <summary>
		/// Checks if a response for a given command is already received
		/// </summary>
		/// <param name="waitedCommandResponse">CommandType whose response is expected</param>
		/// <returns></returns>
		public bool IsResponseReceived(JustinaCommands expectedCmdResp)
		{
			if (this.justinaCmdAndResp[(int)expectedCmdResp].IsResponseReceived)
				return true;
			else return false;
		}

		public bool IsResponseReceived(JustinaCommands expectedCmdResp, out Response receivedResp)
		{
			receivedResp = null;
			if (this.IsResponseReceived(expectedCmdResp))
			{
				receivedResp = this.justinaCmdAndResp[(int)expectedCmdResp].Response;
				return true;
			}
			else return false;
		}

        public bool IsResponseSuccesfully(JustinaCommands cmdToCheck)
        {
            return this.justinaCmdAndResp[(int)cmdToCheck].Response.Success;
        }

		public bool BLK_alive(string module, int timeOut_ms)
		{
			this.SetupAndSendCommand(JustinaCommands.BLK_alive, module);
			return this.WaitForResponse(JustinaCommands.BLK_alive, timeOut_ms);
		}

		public void ARMS_ra_abspos(double x, double y, double z, double roll, double pitch, double yaw, double elbow)
		{
			string parameters = x.ToString("0.00") + " " + y.ToString("0.00") + " " + z.ToString("0.00") +
				" " + roll.ToString("0.00") + " " + pitch.ToString("0.00") + " " + yaw.ToString("0.00") +
				" " + elbow.ToString("0.00");
			this.SetupAndSendCommand(JustinaCommands.ARMS_ra_abspos, parameters);
		}

		public bool ARMS_ra_abspos(double x, double y, double z, double roll, double pitch, double yaw,
			double elbow, int timeOut_ms)
		{
			this.ARMS_ra_abspos(x, y, z, roll, pitch, yaw, elbow);
			return this.WaitForResponse(JustinaCommands.ARMS_ra_abspos, timeOut_ms);
		}

        public void ARMS_ra_artpos(double q1, double q2, double q3, double q4, double q5, double q6, double q7)
        {
            string parameters = q1.ToString("0.000") + " " + q2.ToString("0.000") + " " + q3.ToString("0.000") + " " +
                q4.ToString("0.000") + " " + q5.ToString("0.000") + " " + q6.ToString("0.000") + " " + q7.ToString("0.000");
            this.SetupAndSendCommand(JustinaCommands.ARMS_ra_artpos, parameters);
        }

        public bool ARMS_ra_artpos(double q1, double q2, double q3, double q4, double q5, double q6, double q7, int timeOut_ms)
        {
            this.ARMS_ra_artpos(q1, q2, q3, q4, q5, q6, q7);
            return this.WaitForResponse(JustinaCommands.ARMS_ra_artpos, timeOut_ms);
        }

		public void ARMS_ra_goto(string predefinedPosition)
		{
			this.SetupAndSendCommand(JustinaCommands.ARMS_ra_goto, predefinedPosition);
			//this.justinaCmdAndResp[(int)JustinaCommands.ARMS_ra_goto].IsResponseReceived = false;
			//this.justinaCmdAndResp[(int)JustinaCommands.ARMS_ra_goto].Command.Parameters = predefinedPosition;
			//this.SendCommand(this.justinaCmdAndResp[(int)JustinaCommands.ARMS_ra_goto].Command);
		}

		public bool ARMS_ra_goto(string predefinedPosition, int timeOut_ms)
		{
			this.ARMS_ra_goto(predefinedPosition);
			return this.WaitForResponse(JustinaCommands.ARMS_ra_goto, timeOut_ms);
		}

		public void ARMS_ra_move(string predefinedMovement)
		{
			this.SetupAndSendCommand(JustinaCommands.ARMS_ra_move, predefinedMovement);
			//this.justinaCmdAndResp[(int)JustinaCommands.ARMS_ra_move].IsResponseReceived = false;
			//this.justinaCmdAndResp[(int)JustinaCommands.ARMS_ra_move].Command.Parameters = predefinedMovement;
			//this.SendCommand(this.justinaCmdAndResp[(int)JustinaCommands.ARMS_ra_move].Command);
		}

		public bool ARMS_ra_move(string predefinedMovement, int timeOut_ms)
		{
			this.ARMS_ra_move(predefinedMovement);
			return this.WaitForResponse(JustinaCommands.ARMS_ra_move, timeOut_ms);
		}

        public void ARMS_la_abspos(double x, double y, double z, double roll, double pitch, double yaw, double elbow)
        {
            string parameters = x.ToString("0.00") + " " + y.ToString("0.00") + " " + z.ToString("0.00") +
                " " + roll.ToString("0.00") + " " + pitch.ToString("0.00") + " " + yaw.ToString("0.00") +
                " " + elbow.ToString("0.00");
            this.SetupAndSendCommand(JustinaCommands.ARMS_la_abspos, parameters);
        }

        public bool ARMS_la_abspos(double x, double y, double z, double roll, double pitch, double yaw, double elbow, int timeOut_ms)
        {
            this.ARMS_la_abspos(x, y, z, roll, pitch, yaw, elbow);
            return this.WaitForResponse(JustinaCommands.ARMS_la_abspos, timeOut_ms);
        }

        public void ARMS_la_artpos(double q1, double q2, double q3, double q4, double q5, double q6, double q7)
        {
            string parameters = q1.ToString("0.000") + " " + q2.ToString("0.000") + " " + q3.ToString("0.000") + " " +
                q4.ToString("0.000") + " " + q5.ToString("0.000") + " " + q6.ToString("0.000") + " " + q7.ToString("0.000");
            this.SetupAndSendCommand(JustinaCommands.ARMS_la_artpos, parameters);
        }

        public bool ARMS_la_artpos(double q1, double q2, double q3, double q4, double q5, double q6, double q7, int timeOut_ms)
        {
            this.ARMS_la_artpos(q1, q2, q3, q4, q5, q6, q7);
            return this.WaitForResponse(JustinaCommands.ARMS_la_artpos, timeOut_ms);
        }

        public void ARMS_la_goto(string predefinedPosition)
        {
            this.SetupAndSendCommand(JustinaCommands.ARMS_la_goto, predefinedPosition);
        }

        public bool ARMS_la_goto(string predefinedPosition, int timeOut_ms)
        {
            this.ARMS_la_goto(predefinedPosition);
            return this.WaitForResponse(JustinaCommands.ARMS_la_goto, timeOut_ms);
        }

        public void ARMS_la_move(string predefinedMovement)
        {
            this.SetupAndSendCommand(JustinaCommands.ARMS_la_move, predefinedMovement);
        }

        public bool ARMS_la_move(string predefinedMovement, int timeOut_ms)
        {
            this.ARMS_la_move(predefinedMovement);
            return this.WaitForResponse(JustinaCommands.ARMS_la_move, timeOut_ms);
        }

        public void ARMS_ra_closegrip()
        {
            this.SetupAndSendCommand(JustinaCommands.ARMS_ra_closegrip, "");
        }

        public bool ARMS_ra_closegrip(int timeOut_ms)
        {
            this.ARMS_ra_closegrip();
            return this.WaitForResponse(JustinaCommands.ARMS_ra_closegrip, timeOut_ms);
        }

		public void ARMS_ra_opengrip()
		{
			this.SetupAndSendCommand(JustinaCommands.ARMS_ra_opengrip, "");
		}

		public bool ARMS_ra_opengrip(int timeOut_ms)
		{
			this.ARMS_ra_opengrip();
			return this.WaitForResponse(JustinaCommands.ARMS_ra_opengrip, timeOut_ms);
		}

        public void ARMS_la_closegrip()
        {
            this.SetupAndSendCommand(JustinaCommands.ARMS_la_closegrip, "");
        }

        public bool ARMS_la_closegrip(int timeOut_ms)
        {
            this.ARMS_la_closegrip();
            return this.WaitForResponse(JustinaCommands.ARMS_la_closegrip, timeOut_ms);
        }

        public void ARMS_la_opengrip()
        {
            this.SetupAndSendCommand(JustinaCommands.ARMS_la_opengrip, "");
        }

        public bool ARMS_la_opengrip(int timeOut_ms)
        {
            this.ARMS_la_opengrip();
            return this.WaitForResponse(JustinaCommands.ARMS_la_opengrip, timeOut_ms);
        }

        public bool ARMS_ra_reachable(double x, double y, double z, double roll, double pitch, double yaw, double elbow, int timeOut_ms)
        {
            string parameters = x.ToString("0.0000") + " " + y.ToString("0.0000") + " " + z.ToString("0.0000") +
                " " + roll.ToString("0.0000") + " " + pitch.ToString("0.0000") + " " + yaw.ToString("0.0000") +
                " " + elbow.ToString("0.0000");
            this.SetupAndSendCommand(JustinaCommands.ARMS_ra_reachable, parameters);
            return this.WaitForResponse(JustinaCommands.ARMS_ra_reachable, timeOut_ms);
        }

        public bool ARMS_la_reachable(double x, double y, double z, double roll, double pitch, double yaw, double elbow, int timeOut_ms)
        {
            string parameters = x.ToString("0.0000") + " " + y.ToString("0.0000") + " " + z.ToString("0.0000") +
                " " + roll.ToString("0.0000") + " " + pitch.ToString("0.0000") + " " + yaw.ToString("0.0000") +
                " " + elbow.ToString("0.0000");
            this.SetupAndSendCommand(JustinaCommands.ARMS_ra_reachable, parameters);
            return this.WaitForResponse(JustinaCommands.ARMS_ra_reachable, timeOut_ms);
        }

        public bool HEAD_followskeleton(int timeOut_ms)
        {
            this.SetupAndSendCommand(JustinaCommands.HEAD_followskeleton, "");
            return this.WaitForResponse(JustinaCommands.HEAD_followskeleton, timeOut_ms);
        }

		public void HEAD_lookat(double pan, double tilt)
		{
			this.justinaCmdAndResp[(int)JustinaCommands.HEAD_lookat].IsResponseReceived = false;
			this.justinaCmdAndResp[(int)JustinaCommands.HEAD_lookat].Command.Parameters = pan.ToString("0.0000") +
				" " + tilt.ToString("0.0000");
			this.SendCommand(this.justinaCmdAndResp[(int)JustinaCommands.HEAD_lookat].Command);
		}

		public bool HEAD_lookat(double pan, double tilt, int timeOut_ms)
		{
			this.HEAD_lookat(pan, tilt);
			return this.WaitForResponse(JustinaCommands.HEAD_lookat, timeOut_ms);
		}

        public bool HEAD_lookat(out double pan, out double tilt, int timeOut_ms)
        {
            pan = 0;
            tilt = 0;
            this.SetupAndSendCommand(JustinaCommands.HEAD_lookat, "");
            if (!this.WaitForResponse(JustinaCommands.HEAD_lookat, timeOut_ms)) return false;

            char[] delimiters = { ' ' };
            string[] parts = this.justinaCmdAndResp[(int)JustinaCommands.HEAD_lookat].Response.Parameters.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

            try
            {
                pan = double.Parse(parts[0]);
                tilt = double.Parse(parts[1]);
            }
            catch
            {
                TextBoxStreamWriter.DefaultLog.WriteLine("CmdMan: Can't parse response from hd_lookat");
                return false;
            }

            return true;
        }

		public void HEAD_search(string whatToSearch, int timeInSeconds)
		{
			this.justinaCmdAndResp[(int)JustinaCommands.HEAD_search].IsResponseReceived = false;
			this.justinaCmdAndResp[(int)JustinaCommands.HEAD_search].Command.Parameters = whatToSearch + " " + timeInSeconds.ToString();
			this.SendCommand(this.justinaCmdAndResp[(int)JustinaCommands.HEAD_search].Command);
		}

		public void HEAD_show(string emotion)
		{
			this.justinaCmdAndResp[(int)JustinaCommands.HEAD_show].IsResponseReceived = false;
			this.justinaCmdAndResp[(int)JustinaCommands.HEAD_show].Command.Parameters = emotion;
			this.SendCommand(this.justinaCmdAndResp[(int)JustinaCommands.HEAD_show].Command);
		}

        public bool HEAD_stopfollowskeleton(int timeOut_ms)
        {
            this.SetupAndSendCommand(JustinaCommands.HEAD_stopfollowskeleton, "");
            return this.WaitForResponse(JustinaCommands.HEAD_stopfollowskeleton, timeOut_ms);
        }

		public bool HEAD_show(string emotion, int timeOut_ms)
		{
			this.HEAD_show(emotion);
			return this.WaitForResponse(JustinaCommands.HEAD_show, timeOut_ms);
		}

		public void MVN_PLN_getclose()
		{
			this.justinaCmdAndResp[(int)JustinaCommands.MVN_PLN_getclose].IsResponseReceived = false;
			this.justinaCmdAndResp[(int)JustinaCommands.MVN_PLN_getclose].Command.Parameters = "";
			this.SendCommand(this.justinaCmdAndResp[(int)JustinaCommands.MVN_PLN_getclose].Command);
		}

		public bool MVN_PLN_getclose(int timeOut_ms)
		{
			this.MVN_PLN_getclose();
			return this.WaitForResponse(JustinaCommands.MVN_PLN_getclose, timeOut_ms);
		}

		public void MVN_PLN_getclose(string location)
		{
			this.SetupAndSendCommand(JustinaCommands.MVN_PLN_getclose, location);
		}

		public bool MVN_PLN_getclose(string location, int timeOut_ms)
		{
			this.MVN_PLN_getclose(location);
			return this.WaitForResponse(JustinaCommands.MVN_PLN_getclose, timeOut_ms);
		}

		public bool MVN_PLN_getclose(string location, bool searchonfloor,int timeOut_ms)
		{
			string parameters = location + " " + searchonfloor.ToString();
			this.SetupAndSendCommand(JustinaCommands.MVN_PLN_getclose, parameters);
			return this.WaitForResponse(JustinaCommands.MVN_PLN_getclose, timeOut_ms); 
		}
		
        public bool MVN_PLN_getclose(double x0, double y0, double z0, double x1, double y1, double z1, int timeOut_ms)
        {
            string parameters = "line " + x0.ToString("0.000") + " " + y0.ToString("0.000") + " " +
                z0.ToString("0.000") + " " + x1.ToString("0.000") + " " + y1.ToString("0.000") + " " + z1.ToString("0.000");
            this.SetupAndSendCommand(JustinaCommands.MVN_PLN_getclose, parameters);
            return this.WaitForResponse(JustinaCommands.MVN_PLN_getclose, timeOut_ms);
        }

		public void MVN_PLN_goto(string goalRoom)
		{
			this.justinaCmdAndResp[(int)JustinaCommands.MVN_PLN_goto].IsResponseReceived = false;
			this.justinaCmdAndResp[(int)JustinaCommands.MVN_PLN_goto].Command.Parameters = goalRoom;
			this.SendCommand(this.justinaCmdAndResp[(int)JustinaCommands.MVN_PLN_goto].Command);
		}

		public bool MVN_PLN_goto(string goalRoom, int timeOut_ms)
		{
			this.MVN_PLN_goto(goalRoom);
			return this.WaitForResponse(JustinaCommands.MVN_PLN_goto, timeOut_ms);
		}

		public void MVN_PLN_goto(RegionType regionType, string goalRegion)
		{
			this.justinaCmdAndResp[(int)JustinaCommands.MVN_PLN_goto].IsResponseReceived = false;
			this.justinaCmdAndResp[(int)JustinaCommands.MVN_PLN_goto].Command.Parameters = regionType.ToString().ToLower() + " " + goalRegion;
			this.SendCommand(this.justinaCmdAndResp[(int)JustinaCommands.MVN_PLN_goto].Command);
		}

		public bool MVN_PLN_goto(RegionType regionType, string goalRegion, int timeOut_ms)
		{
			this.MVN_PLN_goto(regionType, goalRegion);
			return this.WaitForResponse(JustinaCommands.MVN_PLN_goto, timeOut_ms);
		}

		public void MVN_PLN_goto(double x, double y, double angle)
		{
			this.justinaCmdAndResp[(int)JustinaCommands.MVN_PLN_goto].IsResponseReceived = false;
			this.justinaCmdAndResp[(int)JustinaCommands.MVN_PLN_goto].Command.Parameters = "xy" + " " +
				x.ToString("0.0000") + " " + y.ToString("0.0000") + " " + angle.ToString("0.0000");
			this.SendCommand(this.justinaCmdAndResp[(int)JustinaCommands.MVN_PLN_goto].Command);
		}

		public void MVN_PLN_goto(double x, double y)
		{
			this.justinaCmdAndResp[(int)JustinaCommands.MVN_PLN_goto].IsResponseReceived = false;
			this.justinaCmdAndResp[(int)JustinaCommands.MVN_PLN_goto].Command.Parameters = "xy" + " " +
				x.ToString("0.0000") + " " + y.ToString("0.0000");
			this.SendCommand(this.justinaCmdAndResp[(int)JustinaCommands.MVN_PLN_goto].Command);
		}

		public void MVN_PLN_move(double distance, double angle, double time)
		{
			this.justinaCmdAndResp[(int)JustinaCommands.MVN_PLN_move].IsResponseReceived = false;
			this.justinaCmdAndResp[(int)JustinaCommands.MVN_PLN_move].Command.Parameters = distance.ToString("0.0000") +
				" " + angle.ToString("0.0000") + " " + time.ToString("0.0000");
			this.SendCommand(this.justinaCmdAndResp[(int)JustinaCommands.MVN_PLN_move].Command);
		}

		public bool MVN_PLN_move(double distance, double angle, double time, int timeOut_ms)
		{
			this.MVN_PLN_move(distance, angle, time);
			return this.WaitForResponse(JustinaCommands.MVN_PLN_move, timeOut_ms);
		}

		public void MVN_PLN_move(double distance, double angle)
		{
			this.justinaCmdAndResp[(int)JustinaCommands.MVN_PLN_move].IsResponseReceived = false;
			this.justinaCmdAndResp[(int)JustinaCommands.MVN_PLN_move].Command.Parameters = distance.ToString("0.0000") +
				" " + angle.ToString("0.0000");
			this.SendCommand(this.justinaCmdAndResp[(int)JustinaCommands.MVN_PLN_move].Command);
		}

		public bool MVN_PLN_move(double distance, double angle, int timeOut_ms)
		{
			this.MVN_PLN_move(distance, angle);
			return this.WaitForResponse(JustinaCommands.MVN_PLN_move, timeOut_ms);
		}

		public void MVN_PLN_move(double distance)
		{
			this.justinaCmdAndResp[(int)JustinaCommands.MVN_PLN_move].IsResponseReceived = false;
			this.justinaCmdAndResp[(int)JustinaCommands.MVN_PLN_move].Command.Parameters = distance.ToString("0.0000");
			this.SendCommand(this.justinaCmdAndResp[(int)JustinaCommands.MVN_PLN_move].Command);
		}

		public bool MVN_PLN_move(double distance, int timeOut_ms)
		{
			this.MVN_PLN_move(distance);
			return this.WaitForResponse(JustinaCommands.MVN_PLN_move, timeOut_ms);
		}

        public void MVN_PLN_obstacle(string options)
        {
            this.SetupAndSendCommand(JustinaCommands.MVN_PLN_obstacle, options);
        }

        public bool MVN_PLN_obstacle(string options, int timeOut_ms)
        {
            this.MVN_PLN_obstacle(options);
            return this.WaitForResponse(JustinaCommands.MVN_PLN_obstacle, timeOut_ms);
        }

		public bool MVN_PLN_obstacle(string options, out List<Vector3> foundobjects, int timeOut_ms)
		{
			foundobjects = new List<Vector3>();
			this.MVN_PLN_obstacle(options);
			if (!this.WaitForResponse(JustinaCommands.MVN_PLN_obstacle, timeOut_ms))
				return false;
			char[] delimiters = { ' ' };
			Vector3 temp = Vector3.Zero;
			string[] parts=this.justinaCmdAndResp[(int)JustinaCommands.MVN_PLN_getclose].Response.Parameters.Split(delimiters,StringSplitOptions.RemoveEmptyEntries);

			if(parts.Length%4!=0)
			{
				TextBoxStreamWriter.DefaultLog.WriteLine("HAL9000CmdMan: Can't parse response from mp_obstacle.");
				return false;
			}
			try
			{
				int i=0;

				for (i = 0; i < parts.Length; i += 4)
				{
					temp.X = Double.Parse(parts[i + 1]);
					temp.Y = Double.Parse(parts[i + 2]);
					temp.Z = Double.Parse(parts[i + 3]);
					foundobjects.Add(temp);
				}
				return true;
 
			}
			catch
			{
				TextBoxStreamWriter.DefaultLog.WriteLine("HAL9000CmdMan: Can't parse response from mp_obstacle.");
				return false;
			}
 
		}

		public void MVN_PLN_position()
		{
			this.justinaCmdAndResp[(int)JustinaCommands.MVN_PLN_position].IsResponseReceived = false;
			this.justinaCmdAndResp[(int)JustinaCommands.MVN_PLN_position].Command.Parameters = "";
			this.SendCommand(this.justinaCmdAndResp[(int)JustinaCommands.MVN_PLN_position].Command);
		}

		public void MVN_PLN_position(double x, double y, double angle)
		{
			this.justinaCmdAndResp[(int)JustinaCommands.MVN_PLN_position].IsResponseReceived = false;
			this.justinaCmdAndResp[(int)JustinaCommands.MVN_PLN_position].Command.Parameters =
				x.ToString("0.0000") + " " + y.ToString("0.0000") + " " + angle.ToString("0.0000");
			this.SendCommand(this.justinaCmdAndResp[(int)JustinaCommands.MVN_PLN_position].Command);
		}

		public void MVN_PLN_position(double x, double y)
		{
			this.justinaCmdAndResp[(int)JustinaCommands.MVN_PLN_position].IsResponseReceived = false;
			this.justinaCmdAndResp[(int)JustinaCommands.MVN_PLN_position].Command.Parameters =
				x.ToString("0.0000") + " " + y.ToString("0.0000");
			this.SendCommand(this.justinaCmdAndResp[(int)JustinaCommands.MVN_PLN_position].Command);
		}

        public bool MVN_PLN_position(out double x, out double y, out double angle, int timeOut_ms)
        {
            x = 0;
            y = 0;
            angle = 0;
            string[] parts;
            char[] delimiters = { ' ' };
            this.SetupAndSendCommand(JustinaCommands.MVN_PLN_position, "");
            if (!this.WaitForResponse(JustinaCommands.MVN_PLN_position, timeOut_ms)) return false;
            parts = this.justinaCmdAndResp[(int)JustinaCommands.OBJ_FNDT_find].Response.Parameters.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            try
            {
                x = double.Parse(parts[0]);
                y = double.Parse(parts[1]);
                angle = double.Parse(parts[2]);
            }
            catch { return false; }
            return true;
        }

		public void MVN_PLN_report()
		{
			this.SetupAndSendCommand(JustinaCommands.MVN_PLN_report, "");
		}

		public bool MVN_PLN_report(int timeOut_ms)
		{
			this.MVN_PLN_report();
			return this.WaitForResponse(JustinaCommands.MVN_PLN_report, timeOut_ms);
		}

		public void MVN_PLN_selfterminate()
		{
			this.justinaCmdAndResp[(int)JustinaCommands.MVN_PLN_selfterminate].IsResponseReceived = false;
			this.justinaCmdAndResp[(int)JustinaCommands.MVN_PLN_selfterminate].Command.Parameters = "";
			this.SendCommand(this.justinaCmdAndResp[(int)JustinaCommands.MVN_PLN_selfterminate].Command);
			this.TerminateProcess("MotionPlannerRevolutions.exe", 1000);
		}

        public void MVN_PLN_setspeeds(byte ls, byte rs)
        {
            this.SetupAndSendCommand(JustinaCommands.MVN_PLN_setspeeds, ls.ToString() + " " + rs.ToString());
        }

		public void MVN_PLN_startfollowhuman()
		{
			this.justinaCmdAndResp[(int)JustinaCommands.MVN_PLN_startfollowhuman].IsResponseReceived = false;
			this.SendCommand(this.justinaCmdAndResp[(int)JustinaCommands.MVN_PLN_startfollowhuman].Command);
		}

        public void MVN_PLN_startfollowhuman(string personToFollow)
        {
            this.SetupAndSendCommand(JustinaCommands.MVN_PLN_startfollowhuman, personToFollow);
        }

		public void MVN_PLN_stopfollowhuman()
		{
			this.justinaCmdAndResp[(int)JustinaCommands.MVN_PLN_stopfollowhuman].IsResponseReceived = false;
			this.SendCommand(this.justinaCmdAndResp[(int)JustinaCommands.MVN_PLN_stopfollowhuman].Command);
		}

        public bool OBJ_FNDT_find(string objToFind, out double x, out double y, out double z, int timeOut_ms)
        {
            x = 0;
            y = 0;
            z = 0;
            string[] parts;
            char[] delimiters = { ' ' };
            this.SetupAndSendCommand(JustinaCommands.OBJ_FNDT_find, objToFind);
            if (!this.WaitForResponse(JustinaCommands.OBJ_FNDT_find, timeOut_ms)) return false;
            parts = this.justinaCmdAndResp[(int)JustinaCommands.OBJ_FNDT_find].Response.Parameters.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            try
            {
                x = double.Parse(parts[0]) / 1000;
                y = double.Parse(parts[1]) / 1000;
                z = double.Parse(parts[2]) / 1000;
            }
            catch { return false; }
            return true;
        }

        public bool OBJ_FNDT_find(string objToFind, out List<string> foudnObj, int timeOut_ms)
        {
            foudnObj = new List<string>();
            this.SetupAndSendCommand(JustinaCommands.OBJ_FNDT_find, objToFind);
            if (!this.WaitForResponse(JustinaCommands.OBJ_FNDT_find, timeOut_ms)) return false;
            char[] delimiters = {' '};
            string[] parts = this.justinaCmdAndResp[(int)JustinaCommands.OBJ_FNDT_find].Response.Parameters.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            try
            {
                for (int i = 0; i < parts.Length; i += 4)
                    foudnObj.Add(parts[i]);
            }
            catch { return false; }
            return true;
        }

        public bool OBJ_FNDT_findontable(string objstring, out List<string> foudnObj, int timeOut_ms)
        {
            foudnObj = new List<string>();
            this.SetupAndSendCommand(JustinaCommands.OBJ_FNDT_findontable, objstring);
            if (!this.WaitForResponse(JustinaCommands.OBJ_FNDT_findontable, timeOut_ms)) return false;
            char[] delimiters = { ' ' };
            string[] parts = this.justinaCmdAndResp[(int)JustinaCommands.OBJ_FNDT_findontable].Response.Parameters.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            try
            {
                for (int i = 0; i < parts.Length; i += 4)
                    foudnObj.Add(parts[i]);
            }
            catch { return false; }
            return true;
        }

        public bool OBJ_FNDT_findedge(double angleRads, out double x0, out double y0, out double z0, out double x1,
            out double y1, out double z1, int timeOut_ms)
        {
            x0 = 0;
            y0 = 0;
            z0 = 0;
            x1 = 1;
            y1 = 0;
            z1 = 0;

            this.SetupAndSendCommand(JustinaCommands.OBJ_FNDT_findedge, (angleRads * 180 / Math.PI).ToString("0.00"));
            if (!this.WaitForResponse(JustinaCommands.OBJ_FNDT_findedge, timeOut_ms)) return false;

            char[] delimiters = { ' ' };
            string[] parts = this.justinaCmdAndResp[(int)JustinaCommands.OBJ_FNDT_findedge].Response.Parameters.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

            try
            {
                x0 = double.Parse(parts[0]);
                y0 = double.Parse(parts[1]);
                z0 = double.Parse(parts[2]);
                x1 = double.Parse(parts[3]);
                y1 = double.Parse(parts[4]);
                z1 = double.Parse(parts[5]);
            }
            catch
            {
                TextBoxStreamWriter.DefaultLog.WriteLine("CmdMan: Cannot parse response from oft_findedge");
                return false;
            }
            return true;
        }

		public void PRS_FND_sleep(bool enable)
		{
			string parameters;
			if(enable) parameters = "enable";
			else parameters = "disable";
			this.SetupAndSendCommand(JustinaCommands.PRS_FND_sleep, parameters);
		}

		public bool PRS_FND_sleep(bool enable, int timeOut_ms)
		{
			this.PRS_FND_sleep(enable);
			return this.WaitForResponse(JustinaCommands.PRS_FND_sleep, timeOut_ms);
		}

        public bool PRS_FND_amnesia(int timeOut_ms)
        {
            this.SetupAndSendCommand(JustinaCommands.PRS_FND_amnesia, "");
            return this.WaitForResponse(JustinaCommands.PRS_FND_amnesia, timeOut_ms);
        }

		public void SPG_GEN_say(string strToSay)
		{
			this.justinaCmdAndResp[(int)JustinaCommands.SP_GEN_say].IsResponseReceived = false;
			this.justinaCmdAndResp[(int)JustinaCommands.SP_GEN_say].Command.Parameters = strToSay;
			this.SendCommand(this.justinaCmdAndResp[(int)JustinaCommands.SP_GEN_say].Command);
		}

		public bool SPG_GEN_say(string strToSay, int timeOut_ms)
		{
			this.SPG_GEN_say(strToSay);
			return this.WaitForResponse(JustinaCommands.SP_GEN_say, timeOut_ms);
		}

        public bool SPG_GEN_asay(string strToSay, int timeOut_ms)
        {
            this.SetupAndSendCommand(JustinaCommands.SP_GEN_asay, strToSay);
            return this.WaitForResponse(JustinaCommands.SP_GEN_asay, timeOut_ms);
        }

        public bool SPG_GEN_shutup(int timeOut_ms)
        {
            this.SetupAndSendCommand(JustinaCommands.SP_GEN_shutup, "");
            return this.WaitForResponse(JustinaCommands.SP_GEN_shutup, timeOut_ms);
        }

        public bool SP_REC_grammar(string grammarFilePath, int timeOut_ms)
        {
            this.SetupAndSendCommand(JustinaCommands.SP_REC_grammar, grammarFilePath);
            return this.WaitForResponse(JustinaCommands.SP_REC_grammar, timeOut_ms);
        }

		public void ST_PLN_alignhuman(string parameters)
		{
			this.SetupAndSendCommand(JustinaCommands.ST_PLN_alignhuman, parameters);
		}

		public bool ST_PLN_alignhuman(string parameters, int timeOut_ms)
		{
			this.ST_PLN_alignhuman(parameters);
			return this.WaitForResponse(JustinaCommands.ST_PLN_alignhuman, timeOut_ms);
		}

		public void ST_PLN_cleantable()
		{
			this.justinaCmdAndResp[(int)JustinaCommands.ST_PLN_cleanplane].IsResponseReceived = false;
			this.justinaCmdAndResp[(int)JustinaCommands.ST_PLN_cleanplane].Command.Parameters = "";
			this.SendCommand(this.justinaCmdAndResp[(int)JustinaCommands.ST_PLN_cleanplane].Command);
		}

		public bool ST_PLN_cleantable(int timeOut_ms)
		{
			this.ST_PLN_cleantable();
			return this.WaitForResponse(JustinaCommands.ST_PLN_cleanplane, timeOut_ms);
		}

        public bool ST_PLN_deliverobject(string objToDeliver, int timeOut_ms)
        {
            this.SetupAndSendCommand(JustinaCommands.ST_PLN_deliverobject, objToDeliver);
            return this.WaitForResponse(JustinaCommands.ST_PLN_deliverobject, timeOut_ms);
        }

		public void ST_PLN_dopresentation()
		{
			this.SetupAndSendCommand(JustinaCommands.ST_PLN_dopresentation, "");
		}

		public bool ST_PLN_dopresentation(int timeOut_ms)
		{
			this.ST_PLN_dopresentation();
			return this.WaitForResponse(JustinaCommands.ST_PLN_dopresentation, timeOut_ms);
		}

		public void ST_PLN_drop()
		{
			this.justinaCmdAndResp[(int)JustinaCommands.ST_PLN_drop].IsResponseReceived = false;
			this.justinaCmdAndResp[(int)JustinaCommands.ST_PLN_drop].Command.Parameters = "";
			this.SendCommand(this.justinaCmdAndResp[(int)JustinaCommands.ST_PLN_drop].Command);
		}

		public bool ST_PLN_drop(int timetOut_ms)
		{
			this.ST_PLN_drop();
			return this.WaitForResponse(JustinaCommands.ST_PLN_drop, timetOut_ms);
		}

		public void ST_PLN_drop(string whereToDrop)
		{
			this.justinaCmdAndResp[(int)JustinaCommands.ST_PLN_drop].IsResponseReceived = false;
			this.justinaCmdAndResp[(int)JustinaCommands.ST_PLN_drop].Command.Parameters = whereToDrop;
			this.SendCommand(this.justinaCmdAndResp[(int)JustinaCommands.ST_PLN_drop].Command);
		}

		public bool ST_PLN_drop(string whereToDrop, int timetOut_ms)
		{
			this.ST_PLN_drop(whereToDrop);
			return this.WaitForResponse(JustinaCommands.ST_PLN_drop, timetOut_ms);
		}

		public void ST_PLN_findhuman(string humanName, string devices)
		{
			this.SetupAndSendCommand(JustinaCommands.ST_PLN_findhuman, humanName + " " +devices);
		}

		public bool ST_PLN_findhuman(string humanName, string devices, int timeOut_ms, out string foundHumanName)
		{
			foundHumanName = "";
			this.ST_PLN_findhuman(humanName, devices);
			if (!this.WaitForResponse(JustinaCommands.ST_PLN_findhuman, timeOut_ms)) return false;
			foundHumanName = this.justinaCmdAndResp[(int)JustinaCommands.ST_PLN_findhuman].Response.Parameters;
			return true;
		}

		public void ST_PLN_findobject(string objectName, string devices)
		{
			this.justinaCmdAndResp[(int)JustinaCommands.ST_PLN_findobject].IsResponseReceived = false;
			this.justinaCmdAndResp[(int)JustinaCommands.ST_PLN_findobject].Command.Parameters = devices + " " + objectName;
			this.SendCommand(this.justinaCmdAndResp[(int)JustinaCommands.ST_PLN_findobject].Command);
		}

		public bool ST_PLN_findobject(string objectName, string devices, int timeOut_ms)
		{
			this.ST_PLN_findobject(objectName, devices);
			return this.WaitForResponse(JustinaCommands.ST_PLN_findobject, timeOut_ms);
		}

		public bool ST_PLN_findobject(string objectName, string devices, int timeOut_ms, out List<string> foundObjects)
		{
            foundObjects = new List<string>();
			this.ST_PLN_findobject(objectName, devices);
			if (!this.WaitForResponse(JustinaCommands.ST_PLN_findobject, timeOut_ms)) return false;
            char[] delimiters = {' '};
            foundObjects.AddRange(this.justinaCmdAndResp[(int)JustinaCommands.ST_PLN_findobject].Response.Parameters.Split(delimiters, StringSplitOptions.RemoveEmptyEntries));
			return true;
		}

        public bool ST_PLN_findobject(string objectsToFind, int timeOut_ms, out List<string> foundObjects)
        {
            return this.ST_PLN_findobject(objectsToFind, "", timeOut_ms, out foundObjects);
        }

		public void ST_PLN_greethuman(string devices)
		{
			this.SetupAndSendCommand(JustinaCommands.ST_PLN_greethuman, devices);
		}

		public bool ST_PLN_greethuman(string devices, int timeOut_ms)
		{
			this.ST_PLN_greethuman(devices);
			return this.WaitForResponse(JustinaCommands.ST_PLN_greethuman, timeOut_ms);
		}

		public void ST_PLN_rememberhuman(string humanName, string devices)
		{
			this.SetupAndSendCommand(JustinaCommands.ST_PLN_rememberhuman, devices + " " + humanName);
		}

		public bool ST_PLN_rememberhuman(string humanName, string devices, int timeOut_ms)
		{
			this.ST_PLN_rememberhuman(humanName, devices);
			return this.WaitForResponse(JustinaCommands.ST_PLN_rememberhuman, timeOut_ms);
		}

		public void ST_PLN_seehand(string objectToBeRecognized)
		{
			this.SetupAndSendCommand(JustinaCommands.ST_PLN_seehand, objectToBeRecognized);
		}

		public bool ST_PLN_seehand(string objectToBeRecognized, int timeOut_ms)
		{
			this.ST_PLN_seehand(objectToBeRecognized);
			return this.WaitForResponse(JustinaCommands.ST_PLN_seehand, timeOut_ms);
		}

		public void ST_PLN_takeobject()
		{
			this.SetupAndSendCommand(JustinaCommands.ST_PLN_take, "");
		}

		public bool ST_PLN_takeobject(int timeOut_ms)
		{
			this.ST_PLN_takeobject();
			return this.WaitForResponse(JustinaCommands.ST_PLN_take, timeOut_ms);
		}

		public void ST_PLN_takeobject(string objectToTake)
		{
			this.SetupAndSendCommand(JustinaCommands.ST_PLN_take, objectToTake);
		}

		public bool ST_PLN_takeobject(string objectToTake, int timeOut_ms)
		{
			this.ST_PLN_takeobject(objectToTake);
			return this.WaitForResponse(JustinaCommands.ST_PLN_take, timeOut_ms);
		}

        public bool ST_PLN_takeobject(string objectsToTake, out List<string> takenObjects, int timeOut_ms)
        {
            takenObjects = new List<string>();
            this.SetupAndSendCommand(JustinaCommands.ST_PLN_take, objectsToTake);
            if (!this.WaitForResponse(JustinaCommands.ST_PLN_take, timeOut_ms)) return false;
            char[] delimiters = { ' ' };
            string[] parts = this.justinaCmdAndResp[(int)JustinaCommands.ST_PLN_take].Response.Parameters.Split(delimiters,StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in parts)
                takenObjects.Add(s);
            if (takenObjects.Count < 1) return false;
            return true;
        }

        public bool TORSO_abspos(double elevation, double pan, int timeOut_ms)
        {
            this.SetupAndSendCommand(JustinaCommands.TORSO_abspos, elevation.ToString("0.000") + " " + pan.ToString("0.0000"));
            return this.WaitForResponse(JustinaCommands.TORSO_abspos, timeOut_ms);
        }

        public bool TORSO_abspos(out double elevation, out double pan, int timeOut_ms)
        {
            elevation = 0;
            pan = 0;
            this.SetupAndSendCommand(JustinaCommands.TORSO_abspos, "");
            if (!this.WaitForResponse(JustinaCommands.TORSO_abspos, timeOut_ms)) return false;
            char[] delimiters = { ' ' };
            try
            {
                string[] parts = this.justinaCmdAndResp[(int)JustinaCommands.TORSO_abspos].Response.Parameters.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                elevation = double.Parse(parts[0]);
                pan = double.Parse(parts[1]);
            }
            catch
            {
                TextBoxStreamWriter.DefaultLog.WriteLine("CmdMan: Cannot parse response from torso");
                return false;
            }
            return true;
        }

        public bool TORSO_mv(double elevation, double pan, int timeOut_ms)
        {
            this.SetupAndSendCommand(JustinaCommands.TORSO_mv, elevation.ToString("0.000") + " " + pan.ToString("0.0000"));
            return this.WaitForResponse(JustinaCommands.TORSO_mv, timeOut_ms);
        }

        public bool TORSO_relpos(double elevation, double pan, int timeOut_ms)
        {
            this.SetupAndSendCommand(JustinaCommands.TORSO_relpos, elevation.ToString("0.000") + " " + pan.ToString("0.0000"));
            return this.WaitForResponse(JustinaCommands.TORSO_relpos, timeOut_ms);
        }

		public JustinaCmdAndResp[] JustinaCmdAndResps
		{
			get { return this.justinaCmdAndResp; }
		}
	}
}
