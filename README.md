# Student-Event-Sourcing
Just a barebones demo harness to explore event sourcing strategies which will go into my personal private repo project.
I made this for myself but I thought I'd share it as it might help others.

## Getting Started
This project requires postgress. 

Don't worry, I got you bro 🤜🏽

Just run the docker compose file:

```bash
$ docker-compose up
```

This will start up postgress and pgAdmin which you can access via a browser at: http://localhost:8080

Finally run the main app with 

```bash
$ dotnet run event-sourcing
```

You will then be asked if you wish to write to the database or not pressing "w" will write data to the DB. 
Remember you must write events to the DB before you can reconstruct the models. 

This serves as a very simple (incomplete) example. It doesn't use snapshots or anything like that yet.
