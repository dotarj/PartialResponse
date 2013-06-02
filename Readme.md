#ASP.NET Web API Partial Response
Partial Response provides partial response (a.k.a. fields) support for ASP.NET Web API.

##Usage
Register the PartialJsonMediaTypeFormatter in Application_Start (in Global.asax):

```
GlobalConfiguration.Configuration.Formatters.Clear();
GlobalConfiguration.Configuration.Formatters.Add(new PartialJsonMediaTypeFormatter() { IgnoreCase = true });
```

##Understanding the ```fields``` parameter

The ```fields``` parameter filters the API response so that the response only includes a specific set of ```fields```. The fields parameter lets you remove nested properties from an API response and thereby reduce your bandwidth usage.

The following rules explain the supported syntax for the ```fields``` parameter value, which is loosely based on XPath syntax:

* Use a comma-separated list (```fields=a,b```) to select multiple fields.
* Use an asterisk (```fields=*```) as a wildcard to identify all fields.
* Use parentheses (```fields=a(b,c)```) to specify a group of nested properties that will be included in the API response.
* Use a forward slash (```fields=a/b```) to identify a nested property.

In practice, these rules often allow several different ```fields``` parameter values to retrieve the same API response. For example, if you want to retrieve the playlist item ID, title, and position for every item in a playlist, you could use any of the following values:

* ```fields=items/id,playlistItems/snippet/title,playlistItems/snippet/position```
* ```fields=items(id,snippet/title,snippet/position)```
* ```fields=items(id,snippet(title,position))```

**Note:** As with all query parameter values, the fields parameter value must be URL encoded. For better readability, the examples in this document omit the encoding.

**Note:** Due to the relatively slow performance of LINQ to JSON (Json.NET), the usage of PartialJsonMediaTypeFormatter has a performance impact compared to the regular Json.NET serializer. Because of the reduced traffic, the overhead in time could be neglected.

**Note:** For now only JSON is supported, but XML support is in development.