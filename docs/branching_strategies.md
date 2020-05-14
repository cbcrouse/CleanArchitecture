# ![Git Branching](./media/git_branch_icon.png) Branching Strategies

This repository has automated build pipelines for building and testing your code. In order to ensure those pipelines are triggered, it's important to name your branches correctly.

## Pipeline Trigger Branch Patterns

* feature/*
* bugfix/*
* hotfix/*

Example branch names:

* feature/caching-implementation
* bugfix/caching-issue-with-key
* hotfix/null-ref-exception

When you push your branch, builds are automatically triggered so that you can know your code is working before you submit a pull request. However, the pull request still requires a fresh passing build.

---