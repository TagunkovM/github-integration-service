# Security Policy

## Supported versions

This is a portfolio project. Security fixes are applied to the latest version on the `main` branch.

## Reporting a vulnerability

Please do not disclose security vulnerabilities in a public issue.

Use GitHub's private vulnerability reporting feature when it is enabled for this repository. If private reporting is not available, open a public issue containing only a request for a private contact channel and do not include exploit details, credentials, tokens, or other sensitive information.

When reporting privately, include:

- the affected component;
- reproduction steps;
- expected and actual behavior;
- potential impact;
- any suggested mitigation, if known.

## Secrets

Never commit GitHub Personal Access Tokens, database credentials used outside local development, or other secrets. Local `.env` files are intentionally ignored by Git.
