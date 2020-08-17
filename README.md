# NemoMQ

## What is it
A very very simple implementation of a Message Broker system with C# and dotnet core, for educational purposes. 

## Goal
The main goal of this project is to keep the features of the Broker to a bare-minimum and focus on its performance.

## Initial Architecture
The communication between the Broker and its clients is handled through TCP connections. The messages are serialized in the JSON format. Each connection with a client is handled in a separate Thread.

## Basic API
* connect(serverIP, serverPort)
* createQueue(queueName)
* subscribe(queueName, callback)
* publish(queueName, data)

## Milestones
* Create a client implementation in C# and a playground application
* Add performance metrics and load-test the Broker
* Replace JSON (de)serialization with raw strings and check performance gains
* Instead of seperate Thread per client, use Thread-pooling and check performance gains
* Make the broker more stable (handle errors and client disconnects)
* Add some more Broker features, like persistent messages, routing algorithms etc