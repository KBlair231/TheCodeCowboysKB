# The Official Code Cowboys Git Workflow Guide
This guide is intended to help contributors and developers better understand our approach to source control. These are best practices and should be followed in most cases to ensure a clean and simple git history for our codebase.
## Merge in a Finished Feature Branch
### Enable Fast Forwarding For Git pull
If for some reason fast forwarding for git pull is not configured properly, then the commands in the next section will result in errors.

To remedy this, run the following command:
```bash
git config --global pull.ff true # Pull will now fast forward if possible, and if not, it will create a merge commit.
```
### Get Changes From official/dev into origin/dev
Before you can get any changes made to official/dev into our feature branch, you need to update your origin/dev branch.
```bash
git checkout dev
git pull origin dev     # Just in case
git pull official dev   # Bring any new changes from the official repo into your local branch
git push origin dev
```
Your dev and origin/dev branches should now be identical to official/dev.
### Get Changes From official/dev into origin/feature
```bash
git checkout your_feature_branch
git rebase dev # Creates a nice linear commit history
git push origin your_feature_branch
```
At this point origin/feature is ready to merge into official/dev
### Create a PR
Coming soon!