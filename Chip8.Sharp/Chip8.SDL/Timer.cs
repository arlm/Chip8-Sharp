using System;
using SDL2;

namespace Chip8
{
    public class Timer
    {
        //The clock time when the timer started
        private uint startTicks;

        //The ticks stored when the timer was paused
        private uint pausedTicks;

        //The timer status
        private bool paused;
        private bool started;

        //Checks the status of the timer
        public bool IsStarted => started;
        public bool IsPaused => paused && started;


        //Gets the timer's time
        public uint Ticks
        {
            get
            {
                //The actual timer time
                uint time = 0;

                //If the timer is running
                if (started)
                {
                    //If the timer is paused
                    if (paused)
                    {
                        //Return the number of ticks when the timer was paused
                        time = pausedTicks;
                    }
                    else
                    {
                        //Return the current time minus the start time
                        time = SDL.SDL_GetTicks() - startTicks;
                    }
                }

                return time;
            }
        }

        //The various clock actions
        public void Start()
        {
            //Start the timer
            started = true;

            //Unpause the timer
            paused = false;

            //Get the current clock time
            startTicks = SDL.SDL_GetTicks();
            pausedTicks = 0;
        }

        public void Stop()
        {
            //Stop the timer
            started = false;

            //Unpause the timer
            paused = false;

            //Clear tick variables
            startTicks = 0;
            pausedTicks = 0;
        }

        public void Pause()
        {
            //If the timer is running and isn't already paused
            if (started && !paused)
            {
                //Pause the timer
                paused = true;

                //Calculate the paused ticks
                pausedTicks = SDL.SDL_GetTicks() - startTicks;
                startTicks = 0;
            }
        }

        public void Resume()
        {
            //If the timer is running and paused
            if (started && paused)
            {
                //Unpause the timer
                paused = false;

                //Reset the starting ticks
                startTicks = SDL.SDL_GetTicks() - pausedTicks;

                //Reset the paused ticks
                pausedTicks = 0;
            }
        }
    }
}
