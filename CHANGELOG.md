## 0.1.6 (06-12-2021):

Added `net6.0` target.

## 0.1.4 (07.01.2020):

DistributedContextModule now registers RequestPriority as a distributed global.

## 0.1.3 (23-03-2019):

DistributedContextTransport now overwrites already existing headers.

## 0.1.2 (23-03-2019):

Context headers are now set by a transport decorator in order to support context changes from other transport decorators (e.g the one used in tracing module).

## 0.1.0 (16-02-2019): 

Initial prerelease.