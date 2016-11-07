Commands
------------
nuget setApiKey xxx-xxx-xxxx-xxxx

nuget push .\packages\Happer.Extensions.1.0.0.0.nupkg

nuget pack ..\Happer.Extensions\Happer.Extensions\Happer.Extensions.csproj -IncludeReferencedProjects -Symbols -Build -Prop Configuration=Release -OutputDirectory ".\packages"
