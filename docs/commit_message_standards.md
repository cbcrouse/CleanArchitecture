# ![Commits](./media/commits.png) Commit Message Standards

This repository supports automatic semantic versioning. There is a pipeline that triggers when the master branch is updated that will read the commit history and determine the appropriate version to set. Below are the supported commit message formats - if your commit message does not meet the requirements, you will be asked to fix your commit message.

---

## Commit Message Format

The following structure conveys the overall shape of the commit message. Each part of the commit message structure is described below.

```xml
<type>(<scope>): <subject>
<body>
<BLANK LINE>
<footer>
```

### Type

This describes the kind of change that this commit is providing.

Must be one of the following:

```yaml
build: Changes that affect the build system or external dependencies
ci: Changes to the CI configuration files and scripts
config: Changes to the configuration settings
docs: Changes to documentation
feat: A new feature
fix: A bug fix
perf: A code change that improves performance
refactor: A code change that neither fixes a bug nor adds a feature
resolve: Used for cleaning up conflicts in files from merges and rebases.
style: Changes that do not affect the meaning of the code (white-space, formatting, missing semi-colons, comment spelling, etc)
test: Adding new tests or modifying existing tests
```

### Scope

Scope can be anything specifying place of the commit change. This might be the name of an assembly, the name of a service, or some other logical identifier.

The following is the list example scopes:

```yml
Authentication
Infrastructure
User
DbContext
```

### Subject

The subject is a short description of the change. This should give a broad view of the commit.
See the following examples for a better idea:

```yml
# Too Broad
test(Application): Updated All Tests

# Too Long
test(Application): Updated each test to consume a package that was recently updated to a higher framework.

# Just right
test(Application): Updated Test Project Dependencies
```

> The body of the message is where the details can be explained.

### Footer

The footer comes after the body (separated by a blank line). This is a special place to add important information for the changelog generator to emphasize.

#### Breaking changes

All breaking changes have to be mentioned as a breaking change block in the footer, which should start with the text `BREAKING CHANGE:` with a space or newlines. The rest of the commit message is then the description of the change, justification and any migration notes.

#### Notable changes

Several types of changes should be called out like breaking changes. This helps implementors of the service know if there are any important changes to the database, settings, etc. to generate for production releases.

`SCHEMA CHANGE:` A new column or table or proc, etc.

`DATA CHANGE:` Data required to make a new feature work

`SETTING CHANGE:` A new setting in the appsettings.json file required to make a new feature work

`NOTABLE CHANGE:` Any change, like above, but that do not fit into one of those higher-level categories.

#### Referencing issues

In Github, work items can be linked by adding the ID number of the item, preceded by a hash character. Closed work items should be listed on a separate line in the footer prefixed with "Closes" keyword like this:

`Closes #234`

or in case of multiple issues:

`Closes #123, #245, #992`

> 'Closes' is only a convention and is not required to link the work item to the commit.

---

## Examples

Here are a few different examples of appropriate commit messages:

```text
feat(Authorization): Added new 'user_required' policy
Added a new policy that can be used to require a user ID in the sub claim of a token.

Closes #736
```

```text
feat(User): Added support for UserProfile on creation
Added support for adding profile information when creating a user. #breaking

BREAKING CHANGE: When a user is created, a profile image URL is now required.

Closes #875
```

```text
feat(Persistence.User): Add profile table

SCHEMA CHANGE: Added [dbo].[UserProfile]

Closes #366
```

```xml
feat(Persistence.User): Added Login Support

SCHEMA CHANGE: Modified existing [dbo].[User] table to support IdentityUser properties

Closes #268
```

```xml
feat(Authorization): Added Single Sign-On

NOTABLE CHANGE: You can now login with your Google account. If you have an existing account, signing in with your Google account will create a new account.

Closes #122
```

```xml
perf(Workflow): Increased the pipeline build speed

Updated the build and test commands to target the solution file instead of the project files which reduces the overall build time from 3 minutes, down to 1 minute.

#236
```

---
