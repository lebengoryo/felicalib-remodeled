param($installPath, $toolsPath, $package, $project)

$felicalibItem = $project.ProjectItems.Item("felicalib.dll")
$outputProperty = $felicalibItem.Properties.Item("CopyToOutputDirectory")
$outputProperty.Value = 2

$felicalib64Item = $project.ProjectItems.Item("felicalib64.dll")
$outputProperty64 = $felicalib64Item.Properties.Item("CopyToOutputDirectory")
$outputProperty64.Value = 2
