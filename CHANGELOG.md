# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [3.0.1] - 2023-07-19

### Added

- Completed steps' outputs are now displayed as orchestrator custom status

## [3.0.0] - 2023-07-17

### Changed

- All activities now return `PatternActivityResult<TResult>` wrapping the result and an "output" object which is used to create orchestrator output.

## [2.0.0] - 2023-07-05

### Changed

- Framework migrated to isolated-process so it can be used with [all dotnet versions](https://learn.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide#supported-versions).
- Steps configuration is now passed from the orchestrator to activity functions as input.

## [1.0.2] - 2023-06-07

### Fixed

- Steps configuration being lost on framework unloading the orchestrator. The steps are now stored in a durable entity instead of an in-memory dictionary.

## [1.0.0] - 2023-05-12

### Added

- Initial project setup
- Support for function chaining pattern
- Support for fan-out-fan-in pattern
