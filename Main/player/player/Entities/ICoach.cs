﻿using RoboCup.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoboCup.Entities
{
    public interface ICoach
    {
        SeenCoachObject GetSeenCoachObject(string name);
        SeenCoachObject GetBall();
        List<SeenCoachObject> GetMyPlayers(int i);
        List<SeenCoachObject> GetOpponentPlayers();
        SeenCoachObject GetMyPos(int i);
        SeenCoachObject GetPlayer(string name);
		List<SeenCoachObject> GetAlllayers(int i);

	}
}
