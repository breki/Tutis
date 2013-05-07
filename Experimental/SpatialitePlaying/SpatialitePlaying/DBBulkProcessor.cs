using System;
using System.Data;
using System.Data.Common;
using System.Globalization;

namespace SpatialitePlaying
{
    public static class DBBulkProcessor
    {
        public static void BulkInsert (
            DbProviderFactory dbProviderFactory,
            IDbConnection connection,
            string selectCommandText,
            string insertCommandText,
            Action<DataTable> fillTableAction)
        {
            using (IDbTransaction transaction = connection.BeginTransaction ())
            {
                using (DbDataAdapter dataAdapter = dbProviderFactory.CreateDataAdapter ())
                {
                    using (IDbCommand command = connection.CreateCommand ())
                    {
                        command.Transaction = transaction;
                        command.CommandText = selectCommandText;
                        dataAdapter.SelectCommand = (DbCommand)command;

                        using (DbCommandBuilder commandBuilder = dbProviderFactory.CreateCommandBuilder ())
                        {
                            commandBuilder.DataAdapter = dataAdapter;
                            using (dataAdapter.InsertCommand =
                                   (DbCommand)((ICloneable)commandBuilder.GetInsertCommand ()).Clone ())
                            {
                                dataAdapter.InsertCommand.CommandText = insertCommandText;

                                commandBuilder.DataAdapter = null;
                                using (DataTable dataTable = new DataTable ())
                                {
                                    dataTable.Locale = CultureInfo.InvariantCulture;
                                    dataAdapter.Fill (dataTable);

                                    fillTableAction (dataTable);

                                    dataAdapter.Update (dataTable);
                                }
                            }
                        }
                    }
                }

                transaction.Commit ();
            }
        }

        public static void BulkUpdate (
            DbProviderFactory dbProviderFactory,
            IDbConnection connection,
            string selectCommandText,
            string updateCommandText,
            Action<DataTable> fillTableAction)
        {
            using (IDbTransaction transaction = connection.BeginTransaction ())
            {
                using (DbDataAdapter dataAdapter = dbProviderFactory.CreateDataAdapter ())
                {
                    using (IDbCommand command = connection.CreateCommand ())
                    {
                        command.Transaction = transaction;
                        command.CommandText = selectCommandText;
                        dataAdapter.SelectCommand = (DbCommand)command;

                        using (DbCommandBuilder commandBuilder = dbProviderFactory.CreateCommandBuilder ())
                        {
                            commandBuilder.DataAdapter = dataAdapter;
                            using (dataAdapter.UpdateCommand =
                                   (DbCommand)((ICloneable)commandBuilder.GetUpdateCommand(false)).Clone ())
                            {
                                dataAdapter.UpdateCommand.CommandText = updateCommandText;
                                //DbParameter dbParameter;
                                //dbParameter = dataAdapter.UpdateCommand.CreateParameter();
                                //dbParameter.ParameterName = "@param1";
                                //dataAdapter.UpdateCommand.Parameters.Add(dbParameter);
                                //dbParameter = dataAdapter.UpdateCommand.CreateParameter();
                                //dbParameter.ParameterName = "@param2";
                                //dataAdapter.UpdateCommand.Parameters.Add(dbParameter);
                                //dbParameter = dataAdapter.UpdateCommand.CreateParameter();
                                //dbParameter.ParameterName = "@param3";
                                //dataAdapter.UpdateCommand.Parameters.Add(dbParameter);

                                commandBuilder.DataAdapter = null;
                                using (DataTable dataTable = new DataTable ())
                                {
                                    dataTable.Locale = CultureInfo.InvariantCulture;
                                    dataAdapter.Fill (dataTable);

                                    fillTableAction (dataTable);

                                    dataAdapter.Update (dataTable);
                                }
                            }
                        }
                    }
                }

                transaction.Commit ();
            }
        }
    }
}