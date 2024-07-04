# Windows Forms UDP Server and Client written in C#
A simple pair of windows forms applications to use as a starting point.
Made using visual studio and C#.

![udpclientserver](https://github.com/ledimestari/udpserverclient/assets/36168743/6aa24635-5bc7-4de1-9686-992a8ce83a6f)

Why?
As I was looking for a simple UDP server and client pair to use as a basis for a personal project of mine I couldn't find any so I made my own, to use as a starting point.
I wanted this to be a very very simple platform and easy to modify or build upon.

## Features

 - Supports multiple clients
 - Assigns an unique id to every client (rolling int by default)
 - Some preset specific replies from the server, eg. sending "hello" prompts the server to reply with "world!"

## To do

 - Client could remember a number of server IPs it has recently been connected to.
 - Server could have a visible list of connected clients.
 - Clients should send a heartbeat.
 - Clients should timeout and drop out from the server if not heard from in a while.
