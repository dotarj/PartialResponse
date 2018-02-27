# ASP.NET Web API Partial Response

[![apache](https://img.shields.io/badge/license-Apache%202-green.svg)](https://raw.githubusercontent.com/dotarj/PartialResponse/master/LICENSE)
[![nuget](https://img.shields.io/nuget/v/WebApi.PartialResponse.svg)](https://www.nuget.org/packages/WebApi.PartialResponse)
[![myget](https://img.shields.io/myget/partialresponse/v/WebApi.PartialResponse.svg)](https://www.myget.org/feed/partialresponse/package/nuget/WebApi.PartialResponse)
[![appveyor](https://ci.appveyor.com/api/projects/status/7ylaqahgotccbdsx?svg=true)](https://ci.appveyor.com/project/dotarj/partialresponse)
[![codecov](https://codecov.io/gh/dotarj/PartialResponse/branch/master/graph/badge.svg)](https://codecov.io/gh/dotarj/PartialResponse)

PartialResponse provides JSON partial response (partial resource) support for ASP.NET Web API. This package is also [available for ASP.NET Core MVC](https://github.com/dotarj/PartialResponse.AspNetCore.Mvc.Formatters.Json/).

## Getting started

First, add a dependency to WebApi.PartialResponse using the NuGet package manager (console):

```
Install-Package WebApi.PartialResponse
```

Then, remove the `JsonMediaTypeFormatter` from the output formatters and add the `PartialJsonMediaTypeFormatter`. The `fields` parameter value, which is used to filter the API response, is case-sensitive by default, but this can be changed using the `PartialJsonMediaTypeFormatter.IgnoreCase` property:

```csharp
configuration.Formatters.Clear();
configuration.Formatters.Add(new PartialJsonMediaTypeFormatter() { IgnoreCase = true });
```

For OWIN self-host or HTTP self-host applications, add the `PartialJsonActionFilter` to the filters:

```csharp
configuration.Filters.Add(new PartialJsonActionFilter());
```

That's it!

## Understanding the fields parameter

The `fields` parameter filters the API response so that the response only includes a specific set of fields. The `fields` parameter lets you remove nested properties from an API response and thereby reduce your bandwidth usage.

The following rules explain the supported syntax for the `fields` parameter value, which is loosely based on XPath syntax:

* Use a comma-separated list (`fields=a,b`) to select multiple fields.
* Use an asterisk (`fields=*`) as a wildcard to identify all fields.
* Use parentheses (`fields=a(b,c)`) to specify a group of nested properties that will be included in the API response.
* Use a forward slash (`fields=a/b`) to identify a nested property.

In practice, these rules often allow several different `fields` parameter values to retrieve the same API response. For example, if you want to retrieve the playlist item ID, title, and position for every item in a playlist, you could use any of the following values:

* `fields=items/id,playlistItems/snippet/title,playlistItems/snippet/position`
* `fields=items(id,snippet/title,snippet/position)`
* `fields=items(id,snippet(title,position))`

**Note:** As with all query parameter values, the 'fields' parameter value must be URL encoded. For better readability, the examples in this document omit the encoding.

**Note:** Due to the relatively slow performance of LINQ to JSON (Json.NET), the usage of PartialJsonOutputFormatter has a performance impact compared to the regular Json.NET serializer. Because of the reduced traffic, the overhead in time could be neglected.