using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Repository
{
    public class CSVCollection
    {
        public IList<Marine> Marines { get; set; }

        public IList<Marine> Preparers { get; set; }
        public IList<MarineDiaries> PreparerDiaries { get; set; }

        public IList<Marine> Certifiers { get; set; }
        public IList<MarineDiaries> CertifierDiaries { get; set; }

        public IList<Marine> Members { get; set; }
        public IList<MarineDiaries> MemberDiaries { get; set; }

        public IList<UnitDiary> Diaries { get; set; }
        public IList<Transaction> Transactions { get; set; }
        public IList<int> Years { get; set; }
        public IList<int> Arucs { get; set; }

        public async Task<CSVCollection> read(string filePath)
        {
            CSVCollection collection = new CSVCollection();
            Hashtable MemberTable = new Hashtable();
            Hashtable CertifierTable = new Hashtable();
            Hashtable PreparerTable = new Hashtable();
            Hashtable DiaryTable = new Hashtable();
            Hashtable MarineTable = new Hashtable();
            Hashtable YearTable = new Hashtable();
            Hashtable ArucTable = new Hashtable();
            Hashtable MemberDiary = new Hashtable();
            Hashtable PrepDiary = new Hashtable();
            Hashtable CertDiary = new Hashtable();

            IList<Marine> CurrentMarines = await new Marine().GetMarines();
            IList<int> CurrentYears = await new Database().GetYears();
            IList<int> CurrentArucs = await new Database().GetArucs();

            foreach (var marine in CurrentMarines)
            {
                MarineTable.Add(marine.EDIPI, marine.LastName);
            }

            foreach (var aruc in CurrentArucs)
            {
                foreach (var year in CurrentYears)
                {
                    AppSettings.Aruc = aruc;
                    AppSettings.Year = year;
                    UnitDiary newDiary = new UnitDiary { Aruc = aruc, Year = year };
                    IList<UnitDiary> unitDiaries = await newDiary.GetAll();
                    foreach (var diary in unitDiaries)
                    {
                        DiaryTable.Add($@"{diary.Number}{diary.Year}{diary.Aruc}", diary.Date);
                    }
                }
            }

            CurrentMarines.Clear();

            if (filePath != null)
            {
                try
                {
                    int rowNumber = 1;
                    using (StreamReader reader = new StreamReader(filePath))
                    {
                        string CSVHeader = reader.ReadLine();
                        IList<Marine> Marines = new List<Marine>();
                        IList<Marine> Members = new List<Marine>();
                        IList<MarineDiaries> MemberDiaries = new List<MarineDiaries>();
                        IList<Marine> Preparers = new List<Marine>();
                        IList<MarineDiaries> PreparerDiaries = new List<MarineDiaries>();
                        IList<Marine> Certifiers = new List<Marine>();
                        IList<MarineDiaries> CertifierDiaries = new List<MarineDiaries>();
                        IList<Transaction> Transactions = new List<Transaction>();
                        IList<UnitDiary> Diaries = new List<UnitDiary>();
                        IList<int> Years = new List<int>();
                        IList<int> Arucs = new List<int>();
                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            string[] values = line.Split('\t');

                            int year = int.Parse(values[2]);
                            int aruc = int.Parse(values[27]);
                            Console.WriteLine($"Reading Row Number {rowNumber}");
                            rowNumber++;
                            UnitDiary diary = new UnitDiary();
                            diary.Aruc = aruc;
                            if (values[4] != string.Empty) { diary.Date = DateTime.Parse(values[4]); }
                            if (values[1] != string.Empty) { diary.CycleNumber = int.Parse(values[1]); }
                            if (values[2] != string.Empty) { diary.Year = int.Parse(values[2]); }
                            if (values[3] != string.Empty) { diary.Number = int.Parse(values[3]); }
                            if (values[0] != string.Empty) { diary.CycleDate = DateTime.Parse(values[0]); }
                            if (values[15] != string.Empty) { diary.Branch = values[15]; }

                            Marine Preparer = new Marine();
                            if (values[5] != string.Empty) { Preparer.EDIPI = int.Parse(values[5]); }
                            if (values[6] != string.Empty) { Preparer.Rank = values[6]; }
                            if (values[7] != string.Empty) { Preparer.LastName = values[7]; }
                            if (values[8] != string.Empty) { Preparer.FirstName = values[8]; }
                            if (values[9] != string.Empty) { Preparer.MI = values[9]; }

                            Marine Certifier = new Marine();
                            if (values[10] != string.Empty) { Certifier.EDIPI = int.Parse(values[10]); }
                            if (values[11] != string.Empty) { Certifier.Rank = values[11]; }
                            if (values[12] != string.Empty) { Certifier.LastName = values[12]; }
                            if (values[13] != string.Empty) { Certifier.FirstName = values[13]; }
                            if (values[14] != string.Empty) { Certifier.MI = values[14]; }

                            Marine Member = new Marine();
                            if (values[16] != string.Empty) { Member.EDIPI = int.Parse(values[16]); }
                            if (values[17] != string.Empty) { Member.Rank = values[17]; }
                            if (values[18] != string.Empty) { Member.LastName = values[18]; }
                            if (values[19] != string.Empty) { Member.FirstName = values[19]; }
                            if (values[20] != string.Empty) { Member.MI = values[20]; }
                            if (values[21] != string.Empty) { Member.PRUC = int.Parse(values[21]); }

                            Transaction transaction = new Transaction();
                            if (values[22] != string.Empty)
                            {
                                try
                                {
                                    transaction.TTC = int.Parse(values[22]);
                                }
                                catch
                                {
                                    transaction.TTC = 000;
                                }
                            }
                            if (values[23] != string.Empty) { transaction.TTS = int.Parse(values[23]); }
                            if (values[3] != string.Empty) { transaction.DiaryNumber = int.Parse(values[3]); }
                            if (values[24] != string.Empty) { transaction.TransactionErrorCode = values[24]; }
                            if (values[25] != string.Empty) { transaction.EnglishStatement = values[25].TrimEnd(' '); }
                            if (values[26] != string.Empty) { transaction.HistoryStatement = values[26].TrimEnd(' '); }
                            if (values[28] != string.Empty) { transaction.DocumentRequired = values[28].TrimEnd(' '); }
                            transaction.ARUC = aruc;
                            transaction.DiaryYear = year;
                            transaction.Member = Member;
                            transaction.Preparer = Preparer;
                            transaction.Certifier = Certifier;
                            Transactions.Add(transaction);
                            diary.Certifier = $@"{Certifier.Rank} {Certifier.LastName}";

                            if (!YearTable.ContainsKey(year))
                            {
                                Years.Add(year);
                                YearTable.Add(year, year);
                            }
                            if (!ArucTable.ContainsKey(aruc))
                            {
                                Arucs.Add(aruc);
                                ArucTable.Add(aruc, aruc);
                            }
                            if (!MemberTable.ContainsKey(Member.EDIPI))
                            {
                                Members.Add(Member);
                                MemberTable.Add(Member.EDIPI, Member.LastName);
                                if (!MarineTable.ContainsKey(Member.EDIPI))
                                {
                                    Marines.Add(Member);
                                    MarineTable.Add(Member.EDIPI, Member.LastName);
                                }
                            }
                            if (!MemberDiary.ContainsKey($"{Member.EDIPI}{diary.Number}{aruc}"))
                            {
                                MemberDiaries.Add(new MarineDiaries { EDIPI = Member.EDIPI, DiaryNumber = diary.Number, Aruc = aruc, Year = year });
                                MemberDiary.Add($"{Member.EDIPI}{diary.Number}{aruc}", diary.Number);
                            }
                            if (!CertifierTable.ContainsKey(Certifier.EDIPI))
                            {
                                Certifiers.Add(Certifier);
                                CertifierTable.Add(Certifier.EDIPI, Certifier.LastName);
                                if (!MarineTable.ContainsKey(Certifier.EDIPI))
                                {
                                    Marines.Add(Certifier);
                                    MarineTable.Add(Certifier.EDIPI, Certifier.LastName);
                                }
                            }
                            if (!CertDiary.ContainsKey($"{Certifier.EDIPI}{diary.Number}{aruc}"))
                            {
                                CertifierDiaries.Add(new MarineDiaries { EDIPI = Certifier.EDIPI, DiaryNumber = diary.Number, Aruc = aruc, Year = year });
                                CertDiary.Add($"{Certifier.EDIPI}{diary.Number}{aruc}", diary.Number);
                            }
                            if (!PreparerTable.ContainsKey(Preparer.EDIPI))
                            {
                                Preparers.Add(Preparer);
                                PreparerTable.Add(Preparer.EDIPI, Preparer.LastName);
                                if (!MarineTable.ContainsKey(Preparer.EDIPI))
                                {
                                    Marines.Add(Preparer);
                                    MarineTable.Add(Preparer.EDIPI, Preparer.LastName);
                                }
                            }
                            if (!PrepDiary.ContainsKey($"{Preparer.EDIPI}{diary.Number}{aruc}"))
                            {
                                PreparerDiaries.Add(new MarineDiaries { EDIPI = Preparer.EDIPI, DiaryNumber = diary.Number, Aruc = aruc, Year = year });
                                PrepDiary.Add($"{Preparer.EDIPI}{diary.Number}{aruc}", diary.Number);
                            }
                            if (!DiaryTable.ContainsKey($@"{diary.Number}{diary.Year}{diary.Aruc}"))
                            {
                                Diaries.Add(diary);
                                DiaryTable.Add($@"{diary.Number}{diary.Year}{diary.Aruc}", diary.Date);
                            }
                        }
                        collection.Years = Years;
                        collection.Arucs = Arucs;
                        collection.Certifiers = Certifiers;
                        collection.CertifierDiaries = CertifierDiaries;
                        collection.Preparers = Preparers;
                        collection.PreparerDiaries = PreparerDiaries;
                        collection.Members = Members;
                        collection.MemberDiaries = MemberDiaries;
                        collection.Diaries = Diaries;
                        collection.Marines = Marines;
                        collection.Transactions = Transactions;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occured while reading the CSV: Error Text: " + ex.Message.ToString(), "Read Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return collection;
            }
            else
            {
                return null;
            }
        }
    }
}
