using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace WeightedLotteryDraft
{
    class Program
    {
        static void Main(string[] args)
        {
            using (StreamWriter sw = new StreamWriter("c:\\temp\\results.txt"))
            {
                for (int j = 0; j < 100; j++)
                {
                    List<Team> teams = InitializeTeams();
                    teams = CalculateWeights(teams);
                    for (int i = 1; i <= 10; i++)
                    {
                        WeightedChanceExecutor weightedChanceExecutor = BuildWCE(teams, sw);

                        weightedChanceExecutor.Execute();

                        teams = RecalculateTeams(teams);
                    }
                    sw.WriteLine();
                }
            }
        }

        static WeightedChanceExecutor BuildWCE(List<Team> teams, StreamWriter sw)
        {
            List<WeightedChanceParam> wcps = new List<WeightedChanceParam>();

            foreach (Team team in teams)
            {
                WeightedChanceParam wcp = new WeightedChanceParam(() =>
                {
                    sw.WriteLine(team.Name + "," + team.Weight + "," + team.Percentage);
                    team.Picked = true;
                }, team.Weight);
                wcps.Add(wcp);
            }

            WeightedChanceExecutor wce = new WeightedChanceExecutor(wcps.ToArray());

            return wce;
        }

        static List<Team> InitializeTeams()
        {
            List<Team> teams = new List<Team>();

            teams.Add(new Team { Name = "Uhh...WAT", Weight = 25, Picked = false });
            teams.Add(new Team { Name = "MDWookies", Weight = 25, Picked = false });
            teams.Add(new Team { Name = "Gordie Howitzers", Weight = 25, Picked = false });
            teams.Add(new Team { Name = "Team Pen15", Weight = 12.5, Picked = false });
            teams.Add(new Team { Name = "RunCMD", Weight = 6.25, Picked = false });
            teams.Add(new Team { Name = "SoPuckingDumb", Weight = 3.125, Picked = false });
            teams.Add(new Team { Name = "MovesLikeJagr", Weight = .78125, Picked = false });
            teams.Add(new Team { Name = "Tim Meadows All Stars", Weight = .78125, Picked = false });
            teams.Add(new Team { Name = "2 Tkachuks 1 Puck", Weight = .78125, Picked = false });
            teams.Add(new Team { Name = "Gohdes", Weight = .78125, Picked = false });

            return teams;
        }
        static List<Team> CalculateWeights(List<Team> originalTeam)
        {
            List<Team> newList = new List<Team>();
            double totalWeight = 0;

            foreach (var team in originalTeam)
            {
                totalWeight += team.Weight;
            }

            foreach (var team in originalTeam)
            {
                newList.Add(new Team { Name = team.Name, Weight = team.Weight, Percentage = ((team.Weight / totalWeight) * 100), Picked = team.Picked });
            }

            return newList;
        }
        static List<Team> RecalculateTeams(List<Team> originalTeams)
        {
            List<Team> newTeams = new List<Team>();

            foreach (Team team in originalTeams)
            {
                if (team.Picked == false)
                {
                    newTeams.Add(new Team { Name = team.Name, Weight = team.Weight, Picked = false });
                }
            }

            return CalculateWeights(newTeams);
        }
    }

    public class WeightedChanceParam
    {
        public Action Func { get; }
        public double Ratio { get; }

        public WeightedChanceParam(Action func, double ratio)
        {
            Func = func;
            Ratio = ratio;
        }
    }
    public class WeightedChanceExecutor
    {
        public WeightedChanceParam[] Parameters { get; }
        private Random r;

        public double RatioSum
        {
            get { return Parameters.Sum(p => p.Ratio); }
        }

        public WeightedChanceExecutor(params WeightedChanceParam[] parameters)
        {
            Parameters = parameters;
            r = new Random();
        }

        public void Execute()
        {
            double numericValue = r.NextDouble() * RatioSum;

            foreach (var parameter in Parameters)
            {
                numericValue -= parameter.Ratio;

                if (!(numericValue <= 0))
                    continue;

                parameter.Func();
                return;
            }
        }
    }
    public class Team
    {
        public string Name { get; set; }
        public double Weight { get; set; }
        public bool Picked { get; set; }
        public double Percentage { get; set; }
    }
}
