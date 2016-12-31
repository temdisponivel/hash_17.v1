using System;
using System.Collections;
using System.Collections.Generic;
using FH.DataRetrieving;
using Hash17.Programs;
using Hash17.Programs.Implementation;
using UnityEditor;
using UnityEngine;
using Help = Hash17.Programs.Implementation.Help;

namespace Hash17.Utils
{
    public class Hash17DataRetrieverInstance : DataRetrieverInstanceBase
    {
        protected static Hash17DataRetrieverInstance _instance;
        public static Hash17DataRetrieverInstance Instance
        {
            get
            {
                var go = new GameObject("Data-Retriever-Instance");
                _instance = go.AddComponent<Hash17DataRetrieverInstance>();
                return _instance;
            }
        }

        #region Fetch Programs

        public void FetchProgramsInfo(string spreadSheetId)
        {
            StartCoroutine(RunFetchProgramsInfos(spreadSheetId));
        }

        private IEnumerator RunFetchProgramsInfos(string spreadSheetId)
        {
            var results = new List<Program>();

            yield return StartCoroutine(GetData(spreadSheetId, "Programs"));

            Debug.Log("FINISH RETRIEVING FROM GOOGLE");

            if (_spreadSheetResults == null)
            {
                Debug.Log("NULL RETURN - DESTROYING");
                Destroy(gameObject);
                yield break;
            }
            
            AssetDatabase.DeleteAsset("Assets/Resources/Programs/ProgramsCollection");

            for (var i = 0; i < _spreadSheetResults.Length; i++)
            {
                var current = _spreadSheetResults[i];

                var id = current["Id"].ToString();
                var uniqueId = int.Parse(current["UniqueId"].ToString());
                var com = current["Command"].ToString();
                var desc = current["Description"].ToString();
                var usage = current["Usage"].ToString();
                var knownParameters = current["KnownParametersAndOptions"].ToString();
                var availableGame = bool.Parse(current["AvailableInGamePlay"].ToString());

                var realId = (ProgramId) Enum.Parse(typeof (ProgramId), id);
                var prog = GetProgramInstance(realId);
                prog.Command = com;
                prog.UnitqueId = uniqueId;
                prog.Description = desc;
                prog.Usage = usage;
                prog.AvailableInGamePlay = availableGame;
                prog.KnownParametersAndOptions = !knownParameters.StartsWith("--") ? knownParameters.Split('|') : new string[0];

                results.Add(prog);
            }

            var collection = DataRetrieverBase.CreateAsset<ProgramCollection>("Assets/Resources/Programs/", "ProgramsCollection.asset");
            collection.Programs = results;

            AssetDatabase.Refresh();

            Debug.Log("Finished creating and configuring programs data!");
            DestroyImmediate(gameObject);
        }

        private Program GetProgramInstance(ProgramId id)
        {
            Program result = null;
            switch (id)
            {
                case ProgramId.Clear:
                    result = new Clear();
                    break;
                case ProgramId.Cd:
                    result = new Cd();
                    break;
                case ProgramId.Read:
                    result = new Read();
                    break;
                case ProgramId.Search:
                    result = new Search();
                    break;
                case ProgramId.Help:
                    result = new Help();
                    break;
                case ProgramId.Init:
                    result = new Init();
                    break;
                case ProgramId.Connect:
                    result = new Connect();
                    break;
            }
            return result;
        }

        #endregion
    }
}