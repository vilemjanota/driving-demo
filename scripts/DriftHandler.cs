using Godot;

namespace DriftHandlerScript
{
    public class DriftHandler
    {
        public bool IsDrifting
        {
            get
            {
                return (animationPlayer.CurrentAnimation == "DriftRight" || animationPlayer.CurrentAnimation == "DriftLeft");
            }
        }
        public bool ShouldDrift
        {
            get 
            {
                if(Input.IsActionPressed("brake") && (Input.IsActionPressed("move_right") || Input.IsActionPressed("move_left")))
                {
                    return true;
                }
                return false;
            }
        }
        private AnimationPlayer animationPlayer;

        public DriftHandler(AnimationPlayer animationPlayer)
        {
            this.animationPlayer = animationPlayer;
        }

        public void UpdateDrift()
        {
            if(ShouldDrift && !IsDrifting)
            {
                if(Input.IsActionPressed("move_right"))
                {
                    startDriftRight();
                }
                else if(Input.IsActionPressed("move_left"))
                {
                    startDriftLeft();
                }
            }
            else if(!ShouldDrift && IsDrifting)
            {
                if(animationPlayer.CurrentAnimation == "DriftRight")
                {
                    stopDriftRight();
                }
                else if(animationPlayer.CurrentAnimation == "DriftLeft")
                {
                    stopDriftLeft();
                }
            }
        }
        private void startDriftRight()
        {
            animationPlayer.Play("StartDriftRight");
            animationPlayer.Queue("DriftRight");
        }
        private void stopDriftRight()
        {
            animationPlayer.PlayBackwards("StartDriftRight");
        }
        private void startDriftLeft()
        {
            animationPlayer.Play("StartDriftLeft");
            animationPlayer.Queue("DriftLeft");
        }
        private void stopDriftLeft()
        {
            animationPlayer.PlayBackwards("StartDriftLeft");
        }

    }
}