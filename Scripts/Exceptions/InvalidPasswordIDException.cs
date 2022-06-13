using System;

class InvalidPasswordIDException : Exception{
	
	/// <summary>
	/// The id of the password that caused the problem
	/// </summary>
	public String passwordID {
		get;
		private set;
	}

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="value"></param>
	public InvalidPasswordIDException(String value) {

		passwordID = value;
	}

	public override String Message {
		get {
			return "Invalid Password ID: " + passwordID;
		}
	}
}

