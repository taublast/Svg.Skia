﻿namespace Svg.Model
{
    public class AddOvalPathCommand : PathCommand
    {
        public Rect Rect;

        public AddOvalPathCommand(Rect rect)
        {
            Rect = rect;
        }
    }
}
