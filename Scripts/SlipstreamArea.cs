using Godot;

namespace BottomlessPit.Scripts;

public partial class SlipstreamArea : Area2D {
	public override void _Ready() {
		BodyEntered += _OnBodyEntered;
		BodyExited += _OnBodyExited;
	}

	public void _OnBodyEntered(Node otherBody) {
		if (otherBody is TestCharacter character) {
			character.EnterSlipstream();
		}
	}

	public void _OnBodyExited(Node otherBody) {
		if (otherBody is TestCharacter character) {
			character.ExitSlipstream();
		}
	}
}