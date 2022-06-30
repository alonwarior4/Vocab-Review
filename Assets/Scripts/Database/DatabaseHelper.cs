using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System.Text;
using System.IO;
using UnityEngine.Networking;
using System;
using Cysharp.Threading.Tasks;
using System.Threading;


//For use in android be sure to copy file in resource folder 
//then create it first game runs , then use it


public class DatabaseHelper
{   
    IDataReader reader;
    IDbConnection dbCon;
    IDbCommand dbCmd;

    StringBuilder sBuilder;

    string db_Name;
    string tableName;

    public async UniTask Initialize(int maxStringLenght , string db_Name , string tableName , CancellationToken token)
    {
        sBuilder = new StringBuilder(maxStringLenght);
        this.db_Name = db_Name;
        this.tableName = tableName;

#if UNITY_ANDROID && !UNITY_EDITOR
        await CopyDatabaseToGameAssetAsync(token);        
#endif

    } 

    async UniTask CopyDatabaseToGameAssetAsync(CancellationToken token)
    {
        sBuilder.Append(ApplicationPath());
        sBuilder.Append("/");
        sBuilder.Append(db_Name);

        if (!File.Exists(sBuilder.ToString()))
        {
            sBuilder.Clear();

            sBuilder.Append("jar:file://");
            sBuilder.Append(Application.dataPath);
            sBuilder.Append("!/assets/");
            sBuilder.Append(db_Name);

            UnityWebRequest loadDb = UnityWebRequest.Get(sBuilder.ToString());
            await loadDb.SendWebRequest().WithCancellation(token);            

            sBuilder.Clear();

            sBuilder.Append(ApplicationPath());
            sBuilder.Append("/");
            sBuilder.Append(db_Name);

            File.WriteAllBytes(sBuilder.ToString(), loadDb.downloadHandler.data);            
        }

        sBuilder.Clear();
    }

    public async UniTask<Word> GetFullRowAsync(int id , CancellationToken token)
    {
        sBuilder.Clear();

        sBuilder.Append("URI=file:");
        sBuilder.Append(ApplicationPath());
        sBuilder.Append("/");
        sBuilder.Append(db_Name);

        dbCon = new SqliteConnection(sBuilder.ToString());
        dbCon.Open();
        dbCmd = dbCon.CreateCommand();

        sBuilder.Clear();

        sBuilder.Append("SELECT * FROM ");
        sBuilder.Append(tableName);
        sBuilder.Append(" WHERE id = ");
        sBuilder.Append(id);
        dbCmd.CommandText = sBuilder.ToString();        

        Word w = new Word();
        await UniTask.Run(() =>
        {
            using (reader = dbCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    w.id = reader.GetInt16(0);
                    w.word = reader.GetString(1);
                    w.meaning = reader.GetString(2);
                    w.description = reader.GetString(3);
                    w.nextCheckDate = reader.GetString(4);
                    w.repeat = reader.GetInt16(5);
                }
            }

        }, true, token);

        CloseConnection();
        sBuilder.Clear();

        return w;
    }

    public async UniTask ChangeDataAsync(Word current)
    {
        sBuilder.Clear();

        sBuilder.Append("URI=file:");
        sBuilder.Append(ApplicationPath());
        sBuilder.Append("/");
        sBuilder.Append(db_Name);

        dbCon = new SqliteConnection(sBuilder.ToString());
        dbCon.Open();
        dbCmd = dbCon.CreateCommand();

        sBuilder.Clear();

        sBuilder.Append("UPDATE ");
        sBuilder.Append(tableName);
        sBuilder.Append(" SET ");
        sBuilder.Append("word = '"+current.word+"', ");
        sBuilder.Append("meaning = '"+current.meaning+"', ");
        sBuilder.Append("description = '" + current.description + "', ");
        sBuilder.Append("nextcheck = '"+current.nextCheckDate+"', ");
        sBuilder.Append("repeat = "+current.repeat+"");
        sBuilder.Append(" WHERE id = "+current.id+"");

        dbCmd.CommandText = sBuilder.ToString();
        dbCmd.ExecuteNonQuery();

        sBuilder.Clear();
        CloseConnection();
    }

    public async UniTask InsertDataAsync(string word, string mean, string description, string dateText, int repeat)
    {
        if (word == null || word == "")
            return;

        sBuilder.Clear();

        sBuilder.Append("URI=file:");
        sBuilder.Append(ApplicationPath());
        sBuilder.Append("/");
        sBuilder.Append(db_Name);

        dbCon = new SqliteConnection(sBuilder.ToString());
        dbCon.Open();
        dbCmd = dbCon.CreateCommand();

        sBuilder.Clear();

        sBuilder.Append("SELECT word FROM ");
        sBuilder.Append(tableName);
        sBuilder.Append(" WHERE word = ");
        sBuilder.AppendFormat("'{0}'", word);
        dbCmd.CommandText = sBuilder.ToString();
        reader = dbCmd.ExecuteReader();

        int count = 0;
        while (reader.Read())        
            count++;

        if(count > 0)
        {
            sBuilder.Clear();
            CloseConnection();
            return;
        }

        sBuilder.Clear();
        reader.Close();
        reader = null;

        sBuilder.Append("INSERT INTO ");
        sBuilder.Append(tableName);
        sBuilder.Append(" VALUES ");
        sBuilder.Append("(NULL, '" + word + "', '" + mean + "', '" + description + "', '" + dateText + "', " + repeat + ")");
        dbCmd.CommandText = sBuilder.ToString();
        dbCmd.ExecuteNonQuery();

        sBuilder.Clear();
        CloseConnection();
    }

    public async UniTask<DateTime?> GetNearestCheckDate()
    {
        sBuilder.Clear();

        sBuilder.Append("URI=file:");
        sBuilder.Append(ApplicationPath());
        sBuilder.Append("/");
        sBuilder.Append(db_Name);

        dbCon = new SqliteConnection(sBuilder.ToString());

        try
        {
            dbCon.Open();
        }
        catch(Exception ex)
        {
            sBuilder.Clear();
            CloseConnection();
            return null;
        }

        dbCmd = dbCon.CreateCommand();

        sBuilder.Clear();

        sBuilder.Append("SELECT nextcheck FROM ");
        sBuilder.Append(tableName);
        sBuilder.Append(" Order By nextcheck ASC Limit 1");

        dbCmd.CommandText = sBuilder.ToString();

        DateTime nearest = default;
        reader = dbCmd.ExecuteReader();
        while (reader.Read())        
            nearest = DateTime.Parse(reader.GetString(0));        

        sBuilder.Clear();
        CloseConnection();

        return nearest;
    }

    public async UniTask<List<int>> GetIDsWithDaytime(string date , CancellationToken token)
    {
        sBuilder.Clear();

        sBuilder.Append("URI=file:");
        sBuilder.Append(ApplicationPath());
        sBuilder.Append("/");
        sBuilder.Append(db_Name);

        dbCon = new SqliteConnection(sBuilder.ToString());
        dbCon.Open();
        dbCmd = dbCon.CreateCommand();

        sBuilder.Clear();

        sBuilder.Append("SELECT id FROM ");
        sBuilder.Append(tableName);
        sBuilder.Append(" WHERE nextcheck <= ");
        sBuilder.AppendFormat("'{0}'", date);
        
        dbCmd.CommandText = sBuilder.ToString();       

        List<int> ids = new List<int>();
        await UniTask.Run(() =>
        {
            using (reader = dbCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    ids.Add(reader.GetInt16(0));                    
                }
            }

        } , true , token);
       

        CloseConnection();
        sBuilder.Clear();

        return ids;
    }

    void CloseConnection()
    {
        if (dbCon != null)
        {
            dbCon.Close();
            dbCon = null;
        }
        if (reader != null)
        {
            reader.Close();
            reader = null;
        }
        if (dbCmd != null)
        {
            dbCmd.Dispose();
            dbCmd = null;
        }
    }

    string ApplicationPath()
    {
        string path = "";
#if UNITY_EDITOR
        path = Application.dataPath;

#elif UNITY_ANDROID && !UNITY_EDITOR
        path = Application.persistentDataPath;
#endif
        return path;
    }

    public void OnDestroy()
    {
        CloseConnection();
    }
}

