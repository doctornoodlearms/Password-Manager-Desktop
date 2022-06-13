using System;

class FileNotFoundException : Exception {


	/// <summary>
	/// The name of the file that caused the error
	/// </summary>
	public String fileName {

		get;
		private set;
	}

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="file"></param>
	public FileNotFoundException(String file) {

		fileName = file;
	}

	public override String Message {

		get => "File Not Found: " + fileName;
	}
}
