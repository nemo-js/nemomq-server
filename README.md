# NemoMQ

## What is it
A very  simple implementation of a Message Broker system with C# and dotnet core, for educational purposes. 

## Goal
The main goal of this project is to keep the features of the Broker to a bare-minimum and focus on its performance.

## Architecture
The communication between the Broker and its clients is handled through TCP connections. The messages are serialized in a custom byte[] format. Client connections are handled with async/await pattern

## Basic API
* connect(serverIP, serverPort)
* createQueue(queueName)
* subscribe(queueName, callback)
* publish(queueName, data)

## Milestones
* [✔] Create a client implementation in C# and a playground application
* [✔] Add performance metrics and load-test the Broker
* [✔] Replace JSON de/serialization with raw strings and check performance gains
* [✔] Instead of seperate Thread per client, use Thread-pooling and check performance gains
* [✔] Use async/await and check performance gains 
* Make the broker more stable (handle errors and client disconnects)
* Remove the 255 bytes max message size
* Add some more Broker features, like persistent messages, routing algorithms etc
* Creare a rest API exposing Broker status (queues, clients, messages etc)

## Performance Conclusions

* Performance is evaluated as messages per second the broker can serve
* Replacing thread per client with async/await pattern gave a 110% performance boost
* Replacing JSON de/serialization with byte manupulation gave another 200% performance boost