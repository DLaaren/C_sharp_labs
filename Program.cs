namespace lab1
{
    class Program
    {
        abstract class Participant
        {
            protected int _id;
            protected string _name;
            protected List<Participant> _wishlist;

            protected Participant(int id, string name)
            {
                Id = id;
                Name = name;
                Wishlist = new List<Participant>();
            }
            
            public int Id { private set; get; }
            public string Name { private set; get; }
            public List<Participant> Wishlist { private set; get; }

            public void MakeWishlist(List<Participant> listOfParticipants)
            {
                Wishlist = listOfParticipants.OrderBy( _ => Random.Shared.Next()).ToList();
            }
        }

        sealed class Junior : Participant
        {
            public Junior(int id, string name) : base(id, name)
            {}
        }

        sealed class Teamlead : Participant
        {
            public Teamlead(int id, string name) : base(id, name) 
            {}
        }

        sealed class HrManager
        {
            // htab = (participant, his wishlist)
            private Dictionary<Participant, List<Participant>> _wishlists;
            // htab = (junior, teamlead)
            private Dictionary<Participant, Participant> _teamlist;

            public HrManager()
            {
                Wishlists = new Dictionary<Participant, List<Participant>>();
                Teamlist = new Dictionary<Participant, Participant>();
            }
            
            public Dictionary<Participant, List<Participant>> Wishlists { private set; get; }
            public Dictionary<Participant, Participant> Teamlist { private set; get; }

            public void SetParticipantWishlist(Participant participant, List<Participant> wishlist)
            {
                if (!Wishlists.TryAdd(participant, wishlist))
                    Wishlists[participant] = wishlist;
            }
            
            public static void Print2DArray<T>(T[,] matrix)
            {
                for (int i = 0; i < 20; i++)
                {
                    for (int j = 0; j < 20; j++)
                    {
                        Console.Write(matrix[i,j] + "\t");
                    }
                    Console.WriteLine();
                }
            }
            
            public Dictionary<Participant, Participant> MakeTeamLists()
            {
                // hungarian algorithm
                int size = Wishlists.Count / 2;
                Console.WriteLine("Wishlists: {0}", size);
                var wishes = new int[size][]; // array of arrays
                var zerosIdxs = new List<int[]>();
                List<int[]> optimalResults = new List<int[]>();
                // firstly, matrix with costs
                Console.WriteLine("Filling the matrix");
                foreach (var participant in Wishlists.Keys)
                {
                    int idx = participant.Id;
                    var wishTeamers = Wishlists[participant];
                    foreach (var wishTeamer in wishTeamers)
                    {
                        Console.WriteLine("Wishteamer: {0}", wishTeamers.IndexOf(wishTeamer));
                        wishes[idx - 1][wishTeamer.Id - 1] = 20 - wishTeamers.IndexOf(wishTeamer);
                    }
                }
                // check matrix
                // Print2DArray(wishes);

                HungaryAlgorithm();
                
                // Finally, collect the results as teamlist
                foreach (var pair in optimalResults)
                {
                    Participant? p1 = Wishlists.Keys.ToList().Find(p => p.Id == pair[0]);
                    Participant? p2 = Wishlists.Keys.ToList().Find(p => p.Id == pair[1]);
                    Teamlist.Add(p1, p2);
                }
                
                return Teamlist;
                
                // finding the maximum optimal value
                void HungaryAlgorithm()
                {
                    // minus the max in a row and then multiply by -1
                    for (int i = 0; i < size; i++)
                    {
                        int max = wishes[i].Max();
                        for (int j = 0; j < size; j++)
                        {
                            wishes[i][j] = (wishes[i][j] - max) * -1;
                        }
                    }

                    // check matrix
                    // Print2DArray(wishes);

                    RowReduction();

                    // find all '0's
                    FindAllZeros(wishes, zerosIdxs);
                    
                    // fins all non-repeatable '0's from left to right
                    // if its number is not equal to a number of rows
                    // then do the same but from right to left
                    optimalResults = ZerosFromBeginning(zerosIdxs);
                    if (optimalResults.Count != size)
                    {
                        optimalResults = ZerosFromEnd(zerosIdxs);
                        if (optimalResults.Count != size)
                        {
                            // if it is still not equal then continue the algo
                            ColumnReduction();

                            optimalResults = ZerosFromBeginning(zerosIdxs);
                            if (optimalResults.Count != size)
                            {
                                optimalResults = ZerosFromEnd(zerosIdxs);
                                if (optimalResults.Count != size)
                                {
                                    // cross out rows and columns with '0'
                                    // find the min of the rest and minus it from the rest
                                    CrossOutWithZeros(wishes);

                                    FindAllZeros(wishes, zerosIdxs);
                                    optimalResults = ZerosFromBeginning(zerosIdxs);
                                    if (optimalResults.Count != size)
                                    {
                                        optimalResults = ZerosFromEnd(zerosIdxs);
                                        if (optimalResults.Count != size)
                                        {
                                            HungaryAlgorithm();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                void ColumnReduction()
                {
                    for (int j = 0; j < size; j++)
                    {
                        int min = Int32.MaxValue;
                        for (int i = 0; i < size; i++)
                        {
                            min = wishes[i][j] < min ? wishes[i][j] : min;
                            if (min == 0)
                                break;
                        }

                        for (int i = 0; i < size; i++)
                        {
                            wishes[i][j] -= min;
                        }
                    }
                }

                void RowReduction()
                {
                    for (int i = 0; i < size; i++)
                    {
                        int min = wishes[i].Min();
                        for (int j = 0; j < size; j++)
                        {
                            wishes[i][j] -= min;
                        }
                    }
                }

                void FindAllZeros(int[][] array, List<int[]> zeros)
                {
                    for (int i = 0; i < array.Length; i++)
                    {
                        for (int j = 0; j < array[i].Length; j++)
                        {
                            if (array[i][j] == 0)
                                zeros.Add(new[] { i, j });
                        }
                    }
                } 
                
                List<int[]> ZerosFromBeginning(List<int[]> zeros, List<int>? i = null, List<int>? j = null, List<int[]>? uniqueZeros = null)
                {
                    List<int[]> findZero = new List<int[]>();
                    if (i== null)
                        i = new List<int>();
                    if (j == null)
                        j = new List<int>();
                    if (uniqueZeros == null)
                        uniqueZeros = new List<int[]>();
                    
                    // find zeros which are not in i_th row and j_th column
                    findZero.AddRange(zeros.Where(zero => !i.Contains(zero[0]) && !j.Contains(zero[1])));

                    if (findZero.Count > 2)
                    {
                        i.Add(findZero[0][0]);
                        j.Add(findZero[0][1]);
                        uniqueZeros.Add(findZero[0].ToArray());
                        
                        ZerosFromBeginning(findZero, i, j, uniqueZeros);
                    }
                    else 
                        uniqueZeros.AddRange(findZero);
                    return uniqueZeros;
                }

                List<int[]> ZerosFromEnd(List<int[]> zeros, List<int>? i = null, List<int>? j = null, List<int[]>? uniqueZeros = null)
                {
                    List<int[]> findZero = new List<int[]>();
                    if (i== null)
                        i = new List<int>();
                    if (j == null)
                        j = new List<int>();
                    if (uniqueZeros == null)
                        uniqueZeros = new List<int[]>();
                    
                    // find zeros which are not in i_th row and j_th column
                    findZero.AddRange(zeros.Where(zero => !i.Contains(zero[0]) && !j.Contains(zero[1])));

                    if (findZero.Count > 2)
                    {
                        i.Add(findZero[^1][0]);
                        j.Add(findZero[^1][1]);
                        uniqueZeros.Add(findZero[^1].ToArray());
                        
                        ZerosFromEnd(findZero, i, j, uniqueZeros);
                    }
                    else 
                        uniqueZeros.AddRange(findZero);
                    return uniqueZeros;
                }

                void CrossOutWithZeros(int[][] array)
                {
                    // crossing out the rows with two or more '0's 
                    var i = Enumerable.Range(0, array.Length).Where(row => array[row].Count(elem => elem == 0) > 1)
                        .ToList();
                    List<int> j = new List<int>();
                    // crossing out the columns with one or more 0's not including crossed out rows' '0's

                    for (int m = 0; m < array.Length; m++)
                    {
                        int zeros = 0;
                        for (int k = 0; k < array[m].Length; k++)
                        {
                            if (array[m][k] == 0 && !i.Contains(k))
                                zeros += 1;
                        }

                        if (zeros > 0)
                            j.Add(m);
                    }

                    // Find min among the rest elems and minus it from the rest elems
                    int restMin = Int32.MaxValue;
                    for (int k = 0; k < array.Length; k++)
                    {
                        for (int m = 0; m < array[k].Length; m++)
                        {
                            if (!i.Contains(k) && !j.Contains(m) && array[k][m] < restMin)
                                restMin = array[k][m];
                        }
                    }

                    for (int k = 0; k < array.Length; k++)
                    {
                        for (int m = 0; m < array[k].Length; m++)
                        {
                            if (!i.Contains(k) && !j.Contains(m))
                                array[k][m] -= restMin;
                        }
                    }
                }
            }
        }

        sealed class HrDirector
        {
            private Dictionary<Participant, Participant> _teamlist;
            private Dictionary<Participant, double> _satisfactionIndexes;
            private double _meanSatisfactionIndex;

            public HrDirector()
            {
                Teamlist = new Dictionary<Participant, Participant>();
                SatisfactionIndexes = new Dictionary<Participant, double>();
            }
            
            public Dictionary<Participant, Participant> Teamlist { set; get; }
            public Dictionary<Participant, double> SatisfactionIndexes { private set; get; }
            public double MeanSatisfactionIndex { private set; get; }

            private double CalculateSatisfactionIndex(Junior junior)
            {
                double satisfactionIndex = 0;
                List<Participant> juniorsWishlist = junior.Wishlist;
                Teamlead teamlead = (Teamlead)_teamlist[junior];
                int teamleadId = juniorsWishlist.FindLastIndex(p => p == teamlead);
                satisfactionIndex = 20 - teamleadId;
                if (!SatisfactionIndexes.TryAdd(junior, satisfactionIndex))
                    SatisfactionIndexes[junior] = satisfactionIndex;
                return satisfactionIndex;
            }
            
            private double CalculateSatisfactionIndex(Teamlead teamlead)
            {
                double satisfactionIndex = 0;
                List<Participant> teamleadWishlist = teamlead.Wishlist;
                Junior junior = (Junior)_teamlist.FirstOrDefault(x => x.Value == teamlead).Key;
                int juniorId = teamleadWishlist.FindLastIndex(p => p == junior);
                satisfactionIndex = 20 - juniorId;
                if (!SatisfactionIndexes.TryAdd(teamlead, satisfactionIndex))
                    SatisfactionIndexes[teamlead] = satisfactionIndex;
                return satisfactionIndex;
            }

            public double CalculateMeanSatisfactionIndex()
            {
                foreach (var junior in Teamlist.Keys)
                {
                    CalculateSatisfactionIndex((Junior)junior);
                }

                foreach (var teamlead in Teamlist.Values)
                {
                    CalculateSatisfactionIndex((Teamlead)teamlead);
                }

                MeanSatisfactionIndex = SatisfactionIndexes.Values.Sum() / Teamlist.Count; // check count
                return MeanSatisfactionIndex;
            }
        }

        class Hackathon
        {
            private Dictionary<int, string> _htabOfJuniors;
            private Dictionary<int, string> _htabOfTeamleads;

            private List<Junior> _juniors;
            private List<Teamlead> _teamleads;
            
            private HrManager _hrManager;
            private HrDirector _hrDirector;

            public Hackathon()
            {
                // parse csv files an get a htab of juniors and teamleads
                _htabOfJuniors = new Dictionary<int, string>();
                _htabOfTeamleads = new Dictionary<int, string>();
                ParseCsvFile("Resources/Juniors20.csv", _htabOfJuniors);
                ParseCsvFile("Resources/Teamleads20.csv", _htabOfTeamleads);
                
                // allocate all juniors and teamleads 
                _juniors = new List<Junior>();
                for (int i = 1; i <= _htabOfJuniors.Count; i++)
                    _juniors.Add(new Junior(i, _htabOfJuniors[i]));

                _teamleads = new List<Teamlead>();
                for (int i = 1; i <= _htabOfTeamleads.Count; i++)
                    _teamleads.Add(new Teamlead(i, _htabOfTeamleads[i]));

                // allocate HRs
                _hrManager = new HrManager();
                _hrDirector = new HrDirector();
            }

            private void ParseCsvFile(in string filePath, Dictionary<int, string> dict)
            {
                foreach (string line in File.ReadAllLines(filePath))
                {
                    string[] tokens = line.Split(";");
                    // skip first line with attributes
                    if (!Int32.TryParse(tokens[0], out var id))
                        continue;
                    string name = tokens[1];
                    dict.Add(id, name);
                }
            }

            public void HoldEvent()
            {
                // all participants make their wishlists
                _juniors.ForEach(j => j.MakeWishlist(_teamleads.Cast<Participant>().ToList()));
                _teamleads.ForEach(t => t.MakeWishlist(_juniors.Cast<Participant>().ToList()));

                _juniors.ForEach(j => _hrManager.SetParticipantWishlist(j, j.Wishlist));
                _teamleads.ForEach(t => _hrManager.SetParticipantWishlist(t, t.Wishlist));

                _hrDirector.Teamlist = _hrManager.MakeTeamLists();
                _hrDirector.CalculateMeanSatisfactionIndex();
            }

            public double GetMeanSatisfactionIndex()
            {
                return _hrDirector.MeanSatisfactionIndex;
            }
        }



        static void Main(string[] args)
        {
            int hackathonNumber = 1;
            double meanSatisfactionIndexCount = 0;
            Hackathon hackathon = new Hackathon();
            for (int i = 0; i < hackathonNumber; i++)
            {
                hackathon.HoldEvent();
                Console.WriteLine("Mean satisfaction index for {0}th round = {1}", (i+1).ToString(),
                    hackathon.GetMeanSatisfactionIndex());
                meanSatisfactionIndexCount += hackathon.GetMeanSatisfactionIndex();
            }

            Console.WriteLine(
                "Mean satisfaction index for all rounds = " + meanSatisfactionIndexCount / hackathonNumber);
        }
    }
}
