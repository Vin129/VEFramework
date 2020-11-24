using System;

[AttributeUsage(AttributeTargets.Class)]
public class QuickExecuteAttribute:Attribute
{
	public bool CanSearch = true;
	public QuickExecuteAttribute(bool value){
		CanSearch = value;
	}
}
[AttributeUsage(AttributeTargets.Method)]
public class ExecuteMethodAttribute:Attribute
{
	public ExecuteMethodAttribute(){
		
	}
}
