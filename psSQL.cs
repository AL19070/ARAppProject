using UnityEngine;
using Npgsql;
using TMPro;
using System;
using System.Collections.Generic;

public class psSQL : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    private string SERVER = "**.**.**.***;";
    private string PORT = "5432;";
    private string DATABASE = "postgres;";
    private string USERID = "postgres;";
    private string PASSWORD = "password;";

    private string connectionString;

    private List<string> itemNameListCop2 = new List<string>() { "�Z���x�[�����ݏ� 12��", "���O���ݒ���v���X ���� 50��" };
    private List<string> itemNameListCop3 = new List<string>() { "�X�N���[�g�ݒɖ� 36��", "�u�X�R�p��A�� 20��", "�T�N����Q 6��" };
    private List<string> itemNameListCop4 = new List<string>() { "�R�[���b�N 120��", "�A�N�A�i�`�������֔�� 132��", "3A�}�O�l�V�A 90��", "�X���[���b�N�}�O�l�V�E�� 30��" };

    private List<string> itemNameListCop2_A = new List<string>() { "�o�C�G���A�X�s���� 30��", "�V�Z�f�X�� 40��" };
    private List<string> itemNameListCop3_A = new List<string>() { "�C�uA�� 90��", "���L�m����M���ɖ� 60��", "�N�C�b�N���ɖ�DX 20��" };
    private List<string> itemNameListCop4_A = new List<string>() { "�m�[�V���A�Z�g�A�~�m�t�F�� 48��", "�^�C���m�[��A 20��", "�����O��N 20��", "�i������ 48��" };
    void Start()
    {
        //���s��
        connectionString =
          "Server=" + SERVER + 
          "Port=" + PORT +
          "Database=" + DATABASE +
          "User ID=" + USERID +
          "Password=" + PASSWORD;
    }
    public void GetTargetListRank()
    {
        List<Item> targetList = new List<Item>();
        targetList.AddRange(GetTargetListInRank());
        string txt = "";
        for (int i = 0; i < targetList.Count; i++)
        {
            txt += targetList[i].GetItemName() + "," + targetList[i].GetPrice() + "," + targetList[i].GetRank() + "\n";
        }
        text.SetText(txt);
    }
    
    //���K�p
    public List<Item> SGetTargetListFromMainItemNameWithRank(string mainItemName)
    {
        connectionString =
          "Server=" + SERVER + 
          "Port=" + PORT +
          "Database=" + DATABASE +
          "User ID=" + USERID +
          "Password=" + PASSWORD;

        List<Item> targetList = new List<Item>();

        //�����N�t�����邽�߂̕ϐ�
        int valueJudge = 0;
        int rankJudge = 1;

        try
        {
            var dbcon = new NpgsqlConnection(connectionString);
            dbcon.Open();

            NpgsqlCommand dbcmd = dbcon.CreateCommand();

            //�@SELECT��
            string sql =
          "select * from test4 where name = '" + mainItemName + "' " +
          "union " +
          "select * from (select * from test4  where name != '" + mainItemName + "' order by price limit 4) as table2 " +
          "order by price";
            dbcmd.CommandText = sql;

            NpgsqlDataReader reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                string itemName = (string)reader["name"];
                int price = (int)reader["price"];

                Item item = new Item();

                if (valueJudge == 0)
                {
                    valueJudge = price;
                    item.SetRank(rankJudge);
                }
                else if (valueJudge == price)
                {
                    item.SetRank(rankJudge);
                }
                else
                {
                    rankJudge++;
                    valueJudge = price;

                    item.SetRank(rankJudge);
                }
                item.SetItemName(itemName);
                item.SetNumberPerTime((int)reader["numpertime"]);
                item.SetNumberPerDay((int)reader["numperday"]);
                item.SetPrice(price);
                
                string component = "";
                string efficacy = (string)reader["component"];
                var list = efficacy.Split('/');
               
                for (int i = 0; i < list.Length; i++)
                {
                    if(i == (list.Length - 1))
                    {
                        component += list[i];
                    }
                    else
                    {
                        component += list[i] + "\n�@�@�@";
                    }
                }
                item.SetEfficacy(component);

                
                if (mainItemName == itemName)
                {
                    targetList.Insert(0, item);
                }
                else
                {
                    targetList.Add(item);
                }
            }

            // clean up
            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbcon.Close();
        }
        catch (System.Exception e)
        {
            text.SetText(e.ToString());
        }
        return targetList;
    }
    //�{�ԗpmainItemName�͔F���������i�̖��O targetList�̍ŏ��̃C���f�b�N�X�ɒǉ�����悤�ɂ���
    public List<Item> HGetTargetListFromMainItemNameWithRank(string mainItemName)
    {
        connectionString =
          "Server=" + SERVER +  
          "Port=" + PORT +
          "Database=" + DATABASE +
          "User ID=" + USERID +
          "Password=" + PASSWORD;

        List<Item> targetList = new List<Item>();

        //�����N�t�����邽�߂̕ϐ�
        int valueJudge = 0;
        int rankJudge = 1;

        try
        {
            var dbcon = new NpgsqlConnection(connectionString);
            dbcon.Open();

            NpgsqlCommand dbcmd = dbcon.CreateCommand();

            //�@SELECT��
            string sql =
          "select * from iteminfo where name = '" + itemNameListCop2[0] + "' or name = '" + itemNameListCop2[1] +"'" +
          "order by price";
            dbcmd.CommandText = sql;

            NpgsqlDataReader reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                string itemName = (string)reader["name"];
                int price = (int)reader["price"];

                Item item = new Item();

                if (valueJudge == 0)
                {
                    valueJudge = price;
                    item.SetRank(rankJudge);
                }
                else if (valueJudge == price)
                {
                    item.SetRank(rankJudge);
                }
                else
                {
                    rankJudge++;
                    valueJudge = price;

                    item.SetRank(rankJudge);
                }
                item.SetItemName(itemName);
                item.SetNumberPerTime((int)reader["numpertime"]);
                item.SetNumberPerDay((int)reader["numperday"]);
                item.SetPrice(price);
                item.SetVolume((int)reader["volume"]);
                item.SetMaker((string)reader["maker"]);
                item.SetForm((string)reader["form"]);

                string component = "";
                string efficacy = (string)reader["component"];
                var list = efficacy.Split('/');
                
                for (int i = 0; i < list.Length; i++)
                {
                    if (i == (list.Length - 1))
                    {
                        component += list[i];
                    }
                    else
                    {
                        component += list[i] + "\n�@�@�@";
                    }
                }
                item.SetEfficacy(component);

                if (mainItemName == itemName)
                {
                    targetList.Insert(0, item);
                }
                else
                {
                    targetList.Add(item);
                }
            }

            // clean up
            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbcon.Close();
        }
        catch (System.Exception e)
        {
            text.SetText(e.ToString());
        }
        return targetList;
    }
    //�{�ԗpmainItemName�͔F���������i�̖��O targetList�̍ŏ��̃C���f�b�N�X�ɒǉ�����悤�ɂ���
    public List<Item> ThreeGetTargetListFromMainItemNameWithRank(string mainItemName)
    {
        connectionString =
          "Server=" + SERVER +  
          "Port=" + PORT +
          "Database=" + DATABASE +
          "User ID=" + USERID +
          "Password=" + PASSWORD;

        List<Item> targetList = new List<Item>();

        //�����N�t�����邽�߂̕ϐ�
        int valueJudge = 0;
        int rankJudge = 1;

        try
        {
            var dbcon = new NpgsqlConnection(connectionString);
            dbcon.Open();

            NpgsqlCommand dbcmd = dbcon.CreateCommand();

            //�@SELECT��
            string sql =
          "select * from iteminfo where name = '" + itemNameListCop3[0] + "' or name = '" + itemNameListCop3[1] + "'" + " or name = '" + itemNameListCop3[2] + "'" +
          "order by price";
            dbcmd.CommandText = sql;

            NpgsqlDataReader reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                string itemName = (string)reader["name"];
                int price = (int)reader["price"];

                Item item = new Item();

                if (valueJudge == 0)
                {
                    valueJudge = price;
                    item.SetRank(rankJudge);
                }
                else if (valueJudge == price)
                {
                    item.SetRank(rankJudge);
                }
                else
                {
                    rankJudge++;
                    valueJudge = price;

                    item.SetRank(rankJudge);
                }
                item.SetItemName(itemName);
                item.SetNumberPerTime((int)reader["numpertime"]);
                item.SetNumberPerDay((int)reader["numperday"]);
                item.SetPrice(price);
                item.SetVolume((int)reader["volume"]);
                item.SetMaker((string)reader["maker"]);
                item.SetForm((string)reader["form"]);

                string component = "";
                string efficacy = (string)reader["component"];
                var list = efficacy.Split('/');
                
                for (int i = 0; i < list.Length; i++)
                {
                    if (i == (list.Length - 1))
                    {
                        component += list[i];
                    }
                    else
                    {
                        component += list[i] + "\n�@�@�@";
                    }
                }
                item.SetEfficacy(component);

                if (mainItemName == itemName)
                {
                    targetList.Insert(0, item);
                }
                else
                {
                    targetList.Add(item);
                }
            }

            // clean up
            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbcon.Close();
        }
        catch (System.Exception e)
        {
            text.SetText(e.ToString());
        }
        return targetList;
    }
    //�{�ԗpmainItemName�͔F���������i�̖��O targetList�̍ŏ��̃C���f�b�N�X�ɒǉ�����悤�ɂ���
    public List<Item> FourGetTargetListFromMainItemNameWithRank(string mainItemName)
    {
        connectionString =
          "Server=" + SERVER +  
          "Port=" + PORT +
          "Database=" + DATABASE +
          "User ID=" + USERID +
          "Password=" + PASSWORD;

        List<Item> targetList = new List<Item>();

        //�����N�t�����邽�߂̕ϐ�
        int valueJudge = 0;
        int rankJudge = 1;

        try
        {
            var dbcon = new NpgsqlConnection(connectionString);
            dbcon.Open();

            NpgsqlCommand dbcmd = dbcon.CreateCommand();

            //�@SELECT��
            string sql =
          "select * from iteminfo where name = '" + itemNameListCop4[0] + "' or name = '" + itemNameListCop4[1] + "'" + " or name = '" + itemNameListCop4[2] + "'" + " or name = '" + itemNameListCop4[3] + "'" +
          "order by price";
            dbcmd.CommandText = sql;

            NpgsqlDataReader reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                string itemName = (string)reader["name"];
                int price = (int)reader["price"];

                Item item = new Item();

                if (valueJudge == 0)
                {
                    valueJudge = price;
                    item.SetRank(rankJudge);
                }
                else if (valueJudge == price)
                {
                    item.SetRank(rankJudge);
                }
                else
                {
                    rankJudge++;
                    valueJudge = price;

                    item.SetRank(rankJudge);
                }
                item.SetItemName(itemName);
                item.SetNumberPerTime((int)reader["numpertime"]);
                item.SetNumberPerDay((int)reader["numperday"]);
                item.SetPrice(price);
                item.SetVolume((int)reader["volume"]);
                item.SetMaker((string)reader["maker"]);
                item.SetForm((string)reader["form"]);

                string component = "";
                string efficacy = (string)reader["component"];
                var list = efficacy.Split('/');
                
                for (int i = 0; i < list.Length; i++)
                {
                    if (i == (list.Length - 1))
                    {
                        component += list[i];
                    }
                    else
                    {
                        component += list[i] + "\n�@�@�@";
                    }
                }
                item.SetEfficacy(component);

                if (mainItemName == itemName)
                {
                    targetList.Insert(0, item);
                }
                else
                {
                    targetList.Add(item);
                }
            }

            // clean up
            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbcon.Close();
        }
        catch (System.Exception e)
        {
            text.SetText(e.ToString());
        }
        return targetList;
    }
    //�{�ԗpmainItemName�͔F���������i�̖��O targetList�̍ŏ��̃C���f�b�N�X�ɒǉ�����悤�ɂ���
    public List<Item> ATwoGetTargetListFromMainItemNameWithRank(string mainItemName)
    {
        connectionString =
          "Server=" + SERVER +   
          "Port=" + PORT +
          "Database=" + DATABASE +
          "User ID=" + USERID +
          "Password=" + PASSWORD;

        List<Item> targetList = new List<Item>();

        //�����N�t�����邽�߂̕ϐ�
        int valueJudge = 0;
        int rankJudge = 1;

        try
        {
            var dbcon = new NpgsqlConnection(connectionString);
            dbcon.Open();

            NpgsqlCommand dbcmd = dbcon.CreateCommand();

            //�@SELECT��
            string sql =
          "select * from iteminfo where name = '" + itemNameListCop2_A[0] + "' or name = '" + itemNameListCop2_A[1] + "'" +
          "order by price";
            dbcmd.CommandText = sql;

            NpgsqlDataReader reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                string itemName = (string)reader["name"];
                int price = (int)reader["price"];

                Item item = new Item();

                if (valueJudge == 0)
                {
                    valueJudge = price;
                    item.SetRank(rankJudge);
                }
                else if (valueJudge == price)
                {
                    item.SetRank(rankJudge);
                }
                else
                {
                    rankJudge++;
                    valueJudge = price;

                    item.SetRank(rankJudge);
                }
                item.SetItemName(itemName);
                item.SetNumberPerTime((int)reader["numpertime"]);
                item.SetNumberPerDay((int)reader["numperday"]);
                item.SetPrice(price);
                item.SetVolume((int)reader["volume"]);
                item.SetMaker((string)reader["maker"]);
                item.SetForm((string)reader["form"]);

                string component = "";
                string efficacy = (string)reader["component"];
                var list = efficacy.Split('/');
                
                for (int i = 0; i < list.Length; i++)
                {
                    if (i == (list.Length - 1))
                    {
                        component += list[i];
                    }
                    else
                    {
                        component += list[i] + "\n�@�@�@";
                    }
                }
                item.SetEfficacy(component);

                
                if (mainItemName == itemName)
                {
                    targetList.Insert(0, item);
                }
                else
                {
                    targetList.Add(item);
                }
            }

            // clean up
            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbcon.Close();
        }
        catch (System.Exception e)
        {
            text.SetText(e.ToString());
        }
        return targetList;
    }
    //�{�ԗpmainItemName�͔F���������i�̖��O targetList�̍ŏ��̃C���f�b�N�X�ɒǉ�����悤�ɂ���
    public List<Item> AThreeGetTargetListFromMainItemNameWithRank(string mainItemName)
    {
        connectionString =
          "Server=" + SERVER +  
          "Port=" + PORT +
          "Database=" + DATABASE +
          "User ID=" + USERID +
          "Password=" + PASSWORD;

        List<Item> targetList = new List<Item>();

        //�����N�t�����邽�߂̕ϐ�
        int valueJudge = 0;
        int rankJudge = 1;

        try
        {
            var dbcon = new NpgsqlConnection(connectionString);
            dbcon.Open();

            NpgsqlCommand dbcmd = dbcon.CreateCommand();

            //�@SELECT��
            string sql =
          "select * from iteminfo where name = '" + itemNameListCop3_A[0] + "' or name = '" + itemNameListCop3_A[1] + "'" + " or name = '" + itemNameListCop3_A[2] + "'" +
          "order by price";
            dbcmd.CommandText = sql;

            NpgsqlDataReader reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                string itemName = (string)reader["name"];
                int price = (int)reader["price"];

                Item item = new Item();

                if (valueJudge == 0)
                {
                    valueJudge = price;
                    item.SetRank(rankJudge);
                }
                else if (valueJudge == price)
                {
                    item.SetRank(rankJudge);
                }
                else
                {
                    rankJudge++;
                    valueJudge = price;

                    item.SetRank(rankJudge);
                }
                item.SetItemName(itemName);
                item.SetNumberPerTime((int)reader["numpertime"]);
                item.SetNumberPerDay((int)reader["numperday"]);
                item.SetPrice(price);
                item.SetVolume((int)reader["volume"]);
                item.SetMaker((string)reader["maker"]);
                item.SetForm((string)reader["form"]);

                string component = "";
                string efficacy = (string)reader["component"];
                var list = efficacy.Split('/');
                
                for (int i = 0; i < list.Length; i++)
                {
                    if (i == (list.Length - 1))
                    {
                        component += list[i];
                    }
                    else
                    {
                        component += list[i] + "\n�@�@�@";
                    }
                }
                item.SetEfficacy(component);

                if (mainItemName == itemName)
                {
                    targetList.Insert(0, item);
                }
                else
                {
                    targetList.Add(item);
                }
            }

            // clean up
            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbcon.Close();
        }
        catch (System.Exception e)
        {
            text.SetText(e.ToString());
        }
        return targetList;
    }
    //�{�ԗpmainItemName�͔F���������i�̖��O targetList�̍ŏ��̃C���f�b�N�X�ɒǉ�����悤�ɂ���
    public List<Item> AFourGetTargetListFromMainItemNameWithRank(string mainItemName)
    {
        connectionString =
          "Server=" + SERVER + 
          "Port=" + PORT +
          "Database=" + DATABASE +
          "User ID=" + USERID +
          "Password=" + PASSWORD;

        List<Item> targetList = new List<Item>();

        //�����N�t�����邽�߂̕ϐ�
        int valueJudge = 0;
        int rankJudge = 1;

        try
        {
            var dbcon = new NpgsqlConnection(connectionString);
            dbcon.Open();

            NpgsqlCommand dbcmd = dbcon.CreateCommand();

            //�@SELECT��
            string sql =
          "select * from iteminfo where name = '" + itemNameListCop4_A[0] + "' or name = '" + itemNameListCop4_A[1] + "'" + " or name = '" + itemNameListCop4_A[2] + "'" + " or name = '" + itemNameListCop4_A[3] + "'" +
          "order by price";
            dbcmd.CommandText = sql;

            NpgsqlDataReader reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                string itemName = (string)reader["name"];
                int price = (int)reader["price"];

                Item item = new Item();

                if (valueJudge == 0)
                {
                    valueJudge = price;
                    item.SetRank(rankJudge);
                }
                else if (valueJudge == price)
                {
                    item.SetRank(rankJudge);
                }
                else
                {
                    rankJudge++;
                    valueJudge = price;

                    item.SetRank(rankJudge);
                }
                item.SetItemName(itemName);
                item.SetNumberPerTime((int)reader["numpertime"]);
                item.SetNumberPerDay((int)reader["numperday"]);
                item.SetPrice(price);
                item.SetVolume((int)reader["volume"]);
                item.SetMaker((string)reader["maker"]);
                item.SetForm((string)reader["form"]);

                string component = "";
                string efficacy = (string)reader["component"];
                var list = efficacy.Split('/');
                
                for (int i = 0; i < list.Length; i++)
                {
                    if (i == (list.Length - 1))
                    {
                        component += list[i];
                    }
                    else
                    {
                        component += list[i] + "\n�@�@�@";
                    }
                }
                item.SetEfficacy(component);

                
                if (mainItemName == itemName)
                {
                    targetList.Insert(0, item);
                }
                else
                {
                    targetList.Add(item);
                }
            }

            // clean up
            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbcon.Close();
        }
        catch (System.Exception e)
        {
            text.SetText(e.ToString());
        }
        return targetList;
    }
    //�ŏ��ɂȂ�����
    public void FirstConnect()
    {
        connectionString =
          "Server=" + SERVER +  
          "Port=" + PORT +
          "Database=" + DATABASE +
          "User ID=" + USERID +
          "Password=" + PASSWORD;

        try
        {
            var dbcon = new NpgsqlConnection(connectionString);
            dbcon.Open();

            NpgsqlCommand dbcmd = dbcon.CreateCommand();

            //�@SELECT��
            string sql =
          "select name from iteminfo where name = '" + "�V�Z�f�X�� 40��" + "' ";
            dbcmd.CommandText = sql;

            NpgsqlDataReader reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
            }

            // clean up
            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbcon.Close();
        }
        catch (System.Exception e)
        {
            text.SetText(e.ToString());
        }
    }
}

