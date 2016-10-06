CommonServiceLocator and Log4Net get IlMerged

Topshelf uses a specific version of CommonServiceLocator 1.0.0.0

nuget won't update the csproj file for the 32bit version of the host
 so you've got to edit it manually if you get errors about not being able to find assemblies in the 32bit version
 