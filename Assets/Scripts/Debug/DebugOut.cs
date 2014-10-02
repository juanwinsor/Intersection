using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DebugOut	
{
	private static DebugOut instance;
	
	private List<string> debugOutput = new List<string>(10);
	
	public static DebugOut Instance
	{
		get
		{
			if(instance == null)
			{
				instance = new DebugOut();
			}
			
			return instance;
		}
	}
	
	public string getOutputString()
	{
		string output = string.Empty;
		
		//return _debugOutput.ToString();
		for(int i = 0; i < debugOutput.Count; i++)
		{
			output += debugOutput[i] + "\n";
		}
		
		return output;
	}
	
	public void AddDebug(string output)
	{
		//limit it to 10 entries
		if(debugOutput.Count == 10)
		{
			debugOutput.RemoveAt(0);			
		}
		
		debugOutput.Add(output);
	}
}
