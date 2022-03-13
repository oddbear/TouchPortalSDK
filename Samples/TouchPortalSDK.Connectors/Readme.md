# Connectors

To make this work, the set the Connectors sample project as the default startup project, and start the application from Visual Studio.

A random value between 0-100 is set on the ShortId update event.

## Connectors

### Connector without data

Simple connector without data.

When changing the value the `connectors with data` is updated.

One is updated based on it's full id, and one is updated based in it's shortId.

### Connector with data

Connector with data, when changing the value, the `connector without data` is updated, based on the data of the current connector.

- Lower: value updated between 0-50
- Upper: value updated between 50-100

## Files

### Connectors.tpz

Page file that can be imported into Touch Portal for testing.
