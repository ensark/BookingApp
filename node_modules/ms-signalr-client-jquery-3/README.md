# ms-signalr-client
## Unofficial package for the SignalR JS Client with jquery 3.x support

This is a package for Microsoft's [ASP.NET SignalR JavaScript Library](https://github.com/SignalR/SignalR/wiki/SignalR-JS-Client)
with slight [modifications](https://github.com/SignalR/SignalR/compare/dev...PaulGiletich:dev) to support jquery 3.x


(a fork of https://github.com/dfrencham/ms-signalr-client)

Current base signalR client version is **v2.2.2-pre**.

### Usage


```
npm install --save ms-signalr-client-jquery-3
```


Include the library in your page using one of the following methods:

#### ES6 Loader

```
import $ from 'jquery';
import 'ms-signalr-client-jquery-3';
```

#### require JS

```
require('jquery');
require('ms-signalr-client-jquery-3');
```

#### HTML

```
<script src="../node_modules/jquery/dist/jquery.js"></script>
<script src="../jquery.signalR.js"></script>
```

Set up a connection to a signalR host, and Hub. Make sure you update the [address], [port], and [hubname] values to suit your environment.

```
var connection = $.hubConnection('http://[address]:[port]');
var proxy = connection.createHubProxy('[hubname]');

// receives broadcast messages from a hub function, called "broadcastMessage"
proxy.on('broadcastMessage', function(message) {
    console.log(message);
});

// atempt connection, and handle errors
connection.start({ jsonp: true })
.done(function(){ console.log('Now connected, connection ID=' + connection.id); })
.fail(function(){ console.log('Could not connect'); });
}
```

### Tests

To run tests, run the following command:

```
$ npm run test
```

Tests will be run using:

- PhantomJS (headless browser)
- Mocha (test runner)
- Chai (assertion library)

### Version numbering

The build number is a little ahead of the offical jquery.signalr repo. This is due to fixes to this package requiring a package version bump.

### Licence Information

As per the following:

- All client code is Copyright (C) Microsoft Corporation. All rights reserved.

I make no claims on this code
