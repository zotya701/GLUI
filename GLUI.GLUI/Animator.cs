using OpenTK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLUI.GLUI
{
    public class Animator
    {
        private TimeSpan mRemainingTime;
        private Stopwatch mTimer;
        private Vector2 mCurrent;

        public TimeSpan Duration { get; set; }
        public Vector2 Source { get; set; }
        public Vector2 Target { get; set; }

        public bool IsRunning { get { return mTimer.IsRunning; } }

        public Vector2 Current
        {
            get
            {
                mCurrent = Source + (Target - Source) * (float)((mTimer.Elapsed.TotalSeconds + mRemainingTime.TotalSeconds) / Duration.TotalSeconds);

                if(mTimer.Elapsed.TotalSeconds + mRemainingTime.TotalSeconds >= Duration.TotalSeconds)
                {
                    Stop();
                    mCurrent = Target;
                }

                return mCurrent;
            }
        }

        public Animator()
        {
            mTimer = new Stopwatch();
        }

        public void Start(Vector2 source, Vector2 target)
        {
            Source = source;
            Target = target;
            mTimer.Restart();
        }

        public void Stop()
        {
            if (!IsRunning) return;

            mRemainingTime = Duration - (mTimer.Elapsed + mRemainingTime);
            mTimer.Stop();
        }

        public void Invert()
        {
            Stop();
            Start(Target, Source);
        }

        public float GetPercentage()
        {
            return (mCurrent[0] - Source[0]) / (Target[0] - Source[0]);
        }
    }
}
