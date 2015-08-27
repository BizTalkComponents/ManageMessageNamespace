[![Build status](https://ci.appveyor.com/api/projects/status/github/BizTalkComponents/ManageMessageNamespace?branch=master)](https://ci.appveyor.com/api/projects/status/github/BizTalkComponents/ManageMessageNamespace/branch/master)

##Description
ManageMessageNamespace consists of three different pipeline components to add, remove or modify the namespace of a message before it is published in or sent from BizTalk.

Messages are read and written to in a streamed manner which helps keeping performance implications to a minimum even with large messages.

###AddNamespace component
Adds a new namespace to a message.

| Parameter                    | Description                                                               | Type| Validation|
| -----------------------------|---------------------------------------------------------------------------|-----|--------|
|NewNamespace|The new namespace to set.|String|Required|
|ShouldUpdateMessageTypeContext|Specifies wether the message type should be updated with the new namespace.|Bool|Required|
|Namespace form|0 = Unqualified, 1 = Qualified, 2 = Default|Enum (int)|Required|
|XPath|The path to set namespace on. Optional.|String|Optional|

###ModifyNamespace component
Changes a specific namespace on a message.

| Parameter       | Description                         | Type| Validation|
| ----------------|-------------------------------------|-----|-----------|
|NamespaceToModify|The namespace that should be changed.|String|Required|
|NewNamespace|The new namespace.|String|Required|

###RemoveNamespace component
Removes namespace on a message.

_No parameters_