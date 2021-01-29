



![Build Master](https://github.com/markglibres/identityserver4-mongodb-redis/workflows/Build%20Master/badge.svg?branch=master) [![NuGet Badge](https://buildstats.info/nuget/BizzPo.IdentityServer)](https://www.nuget.org/packages/BizzPo.IdentityServer/) [![Docker Hub](https://img.shields.io/badge/docker-ready-blue.svg)](https://hub.docker.com/r/bizzpo/identityserver4/)

# IdentityServer4 with MongoDb & Redis cache
"[IdentityServer](https://github.com/IdentityServer/IdentityServer4) is a free, open source [OpenID Connect](http://openid.net/connect/) and [OAuth 2.0](https://tools.ietf.org/html/rfc6749) framework for [ASP.NET](http://asp.net/) Core and is officially [certified](https://openid.net/certification/) by the [OpenID Foundation](https://openid.net/) and thus spec-compliant and interoperable."

This library does the heavy plumbing for IdentityServer4 with MongoDb and Redis cache. Developers can quickly spin up a docker image and start developing their apps without paying for $$$ on authorization service.

## Features
* MongoDb store
* Redis cache as provided by [IdentityServer4.Contrib.RedisStore](https://github.com/AliBazzi/IdentityServer4.Contrib.RedisStore)
* [User interaction UIs (OpenId)](https://github.com/markglibres/identityserver4-mongodb-redis/wiki/User-interaction-UIs)  with easy to configure email templates
* Easy configuration for [Resource Owner Password grant](https://github.com/markglibres/identityserver4-mongodb-redis/wiki/Resource-Owner-Password-Grant)
* Comes with an optional seed data that you can use to start testing IdentityServer4
* Easy way to seed data

Read more from our wiki page to get started:
* [User interaction UIs (OpenId)](https://github.com/markglibres/identityserver4-mongodb-redis/wiki/User-interaction-UIs)
* [Resource Owner Password grant](https://github.com/markglibres/identityserver4-mongodb-redis/wiki/Resource-Owner-Password-Grant)
