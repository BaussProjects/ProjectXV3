//Project by BaussHacker aka. L33TS

using System;

namespace ProjectX_V3_Auth.Enums
{
	/// <summary>
	/// The authentication status of an account.
	/// </summary>
	public enum AccountStatus : byte
    {
        Banned = 0,
        Invalid_AccountID_Or_Password = 1,
        Ready = 2,
        Point_Card_Expired = 6,
        Monthly_Card_Expired = 7,
        Server_Is_Down = 10,
        Please_Try_Again_Later = 11,
        Account_Banned = 12,
        Server_Is_Busy = 20,
        Server_Is_Busy_Try_Again_Later = 21,
        Account_Locked = 22,
        Account_Not_Activated = 30,
        Failed_To_Activate_Account = 31,
        Invalid_Input = 40,
        Invalid_Info = 41,
        Timed_Out = 42,
        Recheck_Serial_Number = 43,
        Unbound = 46,
        Used_3_Login_Attempts = 51,
        Failed_To_Login = 52,
        Datebase_Error = 54,
        Invalid_Account_ID = 57,
        Servers_Not_Configured = 59,
        Server_Locked = 70,
        Account_Locked_by_Phone = 72
    }
}
