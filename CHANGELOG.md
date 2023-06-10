# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [1.0.2] - 2023-06-07

### Fixed

- Steps configuration being lost on framework unloading the orchestrator. The steps are now stored in a durable entity instead of an in-memory dictionary.

## [1.0.0] - 2023-05-12

### Added

- Initial project setup
- Support for function chaining pattern
- Support for fan-out-fan-in pattern
