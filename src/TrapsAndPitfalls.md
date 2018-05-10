- CommonServiceLocator and Log4Net get IlMerged (handled by the MergeDependencies.targets file)

- Topshelf uses a specific version of CommonServiceLocator 1.0.0.0

- You have to update the dependencies of NServiceBus.Host and NServiceBus.Host32 separately, so they need to be kept in sync.
 