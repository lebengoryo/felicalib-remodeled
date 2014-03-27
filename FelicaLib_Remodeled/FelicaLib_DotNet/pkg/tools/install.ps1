param($installPath, $toolsPath, $package, $project)

$felicalibItem = $project.ProjectItems.Item("felicalib.dll")
$outputProperty = $felicalibItem.Properties.Item("CopyToOutputDirectory")
$outputProperty.Value = 2
