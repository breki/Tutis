OMeta# is structured into 3 main components:

1. Runtime - This is the core runtime used by all OMeta# host language implementations.   
2. HostLanguages - These are the implementations of OMeta# that will one day use several backend host languages (currently it's just C#)
3. Examples - These are (hopefully) working examples that demonstrate how you might use OMeta# in a project.

To get started experimenting with a grammar, set an example project (e.g. Calculator) as your startup project
and run it. This puts you in interpretor mode where you can interactively test a grammar.

Note that in each example Program.cs file, you'll see a "OMetaConsoleProgram.Run<Program>();" entry point. 
You can set any of these sample projects as the startup project and then pass in different options such as 
"OMetaConsoleProgram.Run<Program>(OMetaConsoleOptions.CompileGrammars);" for example to have it automatically 
compile the grammar so as to pick up any modifications.

I didn't use "Solution Folders" in the solution to reflect this since it's not supported in VS2008 Express Edition.

Final note: The README project is the default project since it runs most things as a quick sanity check.
