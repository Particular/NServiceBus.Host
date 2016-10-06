param($installPath, $toolsPath, $package, $project)

if(!$toolsPath){
    $project = Get-Project
}

function Add-StartProgramIfNeeded {
    [xml] $prjXml = Get-Content $project.FullName
    foreach($PropertyGroup in $prjXml.project.ChildNodes)
    {
        if($PropertyGroup.StartAction -ne $null)
        {
            return
        }
    }

    $exeName = "NServiceBus.Host"

    if($package.Id.Contains("Host32"))
    {
        $exeName += "32"
    }

    $exeName += ".exe"

    $propertyGroupElement = $prjXml.CreateElement("PropertyGroup", $prjXml.Project.GetAttribute("xmlns"));
    $startActionElement = $prjXml.CreateElement("StartAction", $prjXml.Project.GetAttribute("xmlns"));
    $propertyGroupElement.AppendChild($startActionElement) | Out-Null
    $propertyGroupElement.StartAction = "Program"
    $startProgramElement = $prjXml.CreateElement("StartProgram", $prjXml.Project.GetAttribute("xmlns"));
    $propertyGroupElement.AppendChild($startProgramElement) | Out-Null
    $propertyGroupElement.StartProgram = "`$(ProjectDir)`$(OutputPath)$exeName"
    $prjXml.project.AppendChild($propertyGroupElement) | Out-Null
    $writerSettings = new-object System.Xml.XmlWriterSettings
    $writerSettings.OmitXmlDeclaration = $false
    $writerSettings.NewLineOnAttributes = $false
    $writerSettings.Indent = $true
    $projectFilePath = Resolve-Path -Path $project.FullName
    $writer = [System.Xml.XmlWriter]::Create($projectFilePath, $writerSettings)
    $prjXml.WriteTo($writer)
    $writer.Flush()
    $writer.Close()
}

$project.Save()

Add-StartProgramIfNeeded