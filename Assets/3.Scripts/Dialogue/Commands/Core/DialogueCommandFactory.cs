using System.Collections.Generic;

public class DialogueCommandFactory
{
    private readonly Dictionary<string, IDialogueCommand> _commands = new();

    public DialogueCommandFactory()
    {
        RegisterDefaultCommands();
    }

    private void RegisterDefaultCommands()
    {
        _commands["ShowText"] = new ShowTextCommand();
        _commands["ShowTextStacked"] = new ShowTextStackedCommand();
        _commands["WaitTimer"] = new WaitTimerCommand();
        _commands["CameraZoomIn"] = new CameraZoomInCommand(false);
        _commands["SCameraZoomIn"] = new CameraZoomInCommand(true);
        _commands["CameraZoomOut"] = new CameraZoomOutCommand();
        _commands["CameraShake"] = new CameraShakeCommand();
        _commands["PlayAnim"] = new PlayAnimCommand();
        _commands["EndScript"] = new EndScriptCommand();
        _commands["ShowChoice"] = new ShowChoiceCommand();
        _commands["WaitClick"] = new WaitClickCommand();
        _commands["ModifyHappyGauge"] = new ModifyHappyGaugeCommand();
        _commands["WaitForInput"] = new WaitForInputCommand();
        _commands["CheckPlayerCondition"] = new CheckPlayerConditionCommand();
        _commands["ShowUI"] = new ShowUICommand();
    }

    public void Register(string eventName, IDialogueCommand command)
    {
        _commands[eventName] = command;
    }

    public IDialogueCommand GetCommand(string eventName)
    {
        if (string.IsNullOrWhiteSpace(eventName))
            return _commands["ShowText"]; 

        return _commands.TryGetValue(eventName, out var cmd) ? cmd : _commands["ShowText"];
    }
}