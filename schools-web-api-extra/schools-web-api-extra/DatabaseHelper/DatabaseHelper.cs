using System.Collections.Generic;
using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;
using schools_web_api_extra.Model;
using Microsoft.Extensions.Configuration;
public class DatabaseHelper
{
    private readonly string _connectionString;

    public DatabaseHelper(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Postgres");
    }

    public void SaveOldSchools(List<NewSchool> oldSchools)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();

            using (var transaction = connection.BeginTransaction())
            {
                foreach (var school in oldSchools)
                {
                    var command = connection.CreateCommand();
                    command.Transaction = transaction;

                    command.CommandText = @"
                        INSERT INTO OldSchools (
                            RspoNumer, Longitude, Latitude, Typ, Nazwa, Miejscowosc, Wojewodztwo, KodPocztowy, 
                            NumerBudynku, Email, Ulica, Poczta, Telefon, NumerLokalu, StatusPublicznosc, 
                            StronaInternetowa, Faks, Gmina, Powiat, Dyrektor, NipPodmiotu, DataZalozenia, 
                            LiczbaUczniow, RegonPodmiotu, DataLikwidacji, JezykiNauczane, TerenySportowe,
                            KategoriaUczniow, StrukturaMiejsce, SpecyfikaPlacowki, RodzajMiejscowosci,
                            OrganProwadzacyNip, OrganProwadzacyTyp, PodmiotNadrzednyTyp, KodTerytorialnyGmina, 
                            OrganProwadzacyGmina, OrganProwadzacyNazwa, OrganProwadzacyRegon, PodmiotNadrzednyRspo, 
                            KodTerytorialnyPowiat, OrganProwadzacyPowiat, PodmiotNadrzednyNazwa, 
                            KodTerytorialnyMiejscowosc, KodTerytorialnyWojewodztwo, OrganProwadzacyWojewodztwo
                        ) VALUES (
                            @RspoNumer, @Longitude, @Latitude, @Typ, @Nazwa, @Miejscowosc, @Wojewodztwo, @KodPocztowy, 
                            @NumerBudynku, @Email, @Ulica, @Poczta, @Telefon, @NumerLokalu, @StatusPublicznosc, 
                            @StronaInternetowa, @Faks, @Gmina, @Powiat, @Dyrektor, @NipPodmiotu, @DataZalozenia, 
                            @LiczbaUczniow, @RegonPodmiotu, @DataLikwidacji, @JezykiNauczane, @TerenySportowe,
                            @KategoriaUczniow, @StrukturaMiejsce, @SpecyfikaPlacowki, @RodzajMiejscowosci,
                            @OrganProwadzacyNip, @OrganProwadzacyTyp, @PodmiotNadrzednyTyp, @KodTerytorialnyGmina, 
                            @OrganProwadzacyGmina, @OrganProwadzacyNazwa, @OrganProwadzacyRegon, @PodmiotNadrzednyRspo, 
                            @KodTerytorialnyPowiat, @OrganProwadzacyPowiat, @PodmiotNadrzednyNazwa, 
                            @KodTerytorialnyMiejscowosc, @KodTerytorialnyWojewodztwo, @OrganProwadzacyWojewodztwo
                        );
                    ";

                    command.Parameters.AddWithValue("RspoNumer", (object?)school.RspoNumer ?? DBNull.Value);
                    command.Parameters.AddWithValue("Longitude", school.Longitude);
                    command.Parameters.AddWithValue("Latitude", school.Latitude);
                    command.Parameters.AddWithValue("Typ", (object?)school.Typ ?? DBNull.Value);
                    command.Parameters.AddWithValue("Nazwa", (object?)school.Nazwa ?? DBNull.Value);
                    command.Parameters.AddWithValue("Miejscowosc", (object?)school.Miejscowosc ?? DBNull.Value);
                    command.Parameters.AddWithValue("Wojewodztwo", (object?)school.Wojewodztwo ?? DBNull.Value);
                    command.Parameters.AddWithValue("KodPocztowy", (object?)school.KodPocztowy ?? DBNull.Value);
                    command.Parameters.AddWithValue("NumerBudynku", (object?)school.NumerBudynku ?? DBNull.Value);
                    command.Parameters.AddWithValue("Email", (object?)school.Email ?? DBNull.Value);
                    command.Parameters.AddWithValue("Ulica", (object?)school.Ulica ?? DBNull.Value);
                    command.Parameters.AddWithValue("Poczta", (object?)school.Poczta ?? DBNull.Value);
                    command.Parameters.AddWithValue("Telefon", (object?)school.Telefon ?? DBNull.Value);
                    command.Parameters.AddWithValue("NumerLokalu", (object?)school.NumerLokalu ?? DBNull.Value);
                    command.Parameters.AddWithValue("StatusPublicznosc", (object?)school.StatusPublicznosc ?? DBNull.Value);
                    command.Parameters.AddWithValue("StronaInternetowa", (object?)school.StronaInternetowa ?? DBNull.Value);
                    command.Parameters.AddWithValue("Faks", (object?)school.Faks ?? DBNull.Value);
                    command.Parameters.AddWithValue("Gmina", (object?)school.Gmina ?? DBNull.Value);
                    command.Parameters.AddWithValue("Powiat", (object?)school.Powiat ?? DBNull.Value);
                    command.Parameters.AddWithValue("Dyrektor", (object?)school.Dyrektor ?? DBNull.Value);
                    command.Parameters.AddWithValue("NipPodmiotu", (object?)school.NipPodmiotu ?? DBNull.Value);
                    command.Parameters.AddWithValue("DataZalozenia", (object?)school.DataZalozenia ?? DBNull.Value);
                    command.Parameters.AddWithValue("LiczbaUczniow", (object?)school.LiczbaUczniow ?? DBNull.Value);
                    command.Parameters.AddWithValue("RegonPodmiotu", (object?)school.RegonPodmiotu ?? DBNull.Value);
                    command.Parameters.AddWithValue("DataLikwidacji", (object?)school.DataLikwidacji ?? DBNull.Value);
                    command.Parameters.AddWithValue("JezykiNauczane", (object?)school.JezykiNauczane ?? DBNull.Value);
                    command.Parameters.AddWithValue("TerenySportowe", (object?)school.TerenySportowe ?? DBNull.Value);
                    command.Parameters.AddWithValue("KategoriaUczniow", (object?)school.KategoriaUczniow ?? DBNull.Value);
                    command.Parameters.AddWithValue("StrukturaMiejsce", (object?)school.StrukturaMiejsce ?? DBNull.Value);
                    command.Parameters.AddWithValue("SpecyfikaPlacowki", (object?)school.SpecyfikaPlacowki ?? DBNull.Value);
                    command.Parameters.AddWithValue("RodzajMiejscowosci", (object?)school.RodzajMiejscowosci ?? DBNull.Value);
                    command.Parameters.AddWithValue("OrganProwadzacyNip", (object?)school.OrganProwadzacyNip ?? DBNull.Value);
                    command.Parameters.AddWithValue("OrganProwadzacyTyp", (object?)school.OrganProwadzacyTyp ?? DBNull.Value);
                    command.Parameters.AddWithValue("PodmiotNadrzednyTyp", (object?)school.PodmiotNadrzednyTyp ?? DBNull.Value);
                    command.Parameters.AddWithValue("KodTerytorialnyGmina", (object?)school.KodTerytorialnyGmina ?? DBNull.Value);
                    command.Parameters.AddWithValue("OrganProwadzacyGmina", (object?)school.OrganProwadzacyGmina ?? DBNull.Value);
                    command.Parameters.AddWithValue("OrganProwadzacyNazwa", (object?)school.OrganProwadzacyNazwa ?? DBNull.Value);
                    command.Parameters.AddWithValue("OrganProwadzacyRegon", (object?)school.OrganProwadzacyRegon ?? DBNull.Value);
                    command.Parameters.AddWithValue("PodmiotNadrzednyRspo", (object?)school.PodmiotNadrzednyRspo ?? DBNull.Value);
                    command.Parameters.AddWithValue("KodTerytorialnyPowiat", (object?)school.KodTerytorialnyPowiat ?? DBNull.Value);
                    command.Parameters.AddWithValue("OrganProwadzacyPowiat", (object?)school.OrganProwadzacyPowiat ?? DBNull.Value);
                    command.Parameters.AddWithValue("PodmiotNadrzednyNazwa", (object?)school.PodmiotNadrzednyNazwa ?? DBNull.Value);
                    command.Parameters.AddWithValue("KodTerytorialnyMiejscowosc", (object?)school.KodTerytorialnyMiejscowosc ?? DBNull.Value);
                    command.Parameters.AddWithValue("KodTerytorialnyWojewodztwo", (object?)school.KodTerytorialnyWojewodztwo ?? DBNull.Value);
                    command.Parameters.AddWithValue("OrganProwadzacyWojewodztwo", (object?)school.OrganProwadzacyWojewodztwo ?? DBNull.Value);


                    command.ExecuteNonQuery();
                }

                transaction.Commit();
            }
        }
    }

    public void DeleteAllOldSchools()
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();

            using (var transaction = connection.BeginTransaction())
            {
                var command = connection.CreateCommand();
                command.Transaction = transaction;

                command.CommandText = @"TRUNCATE TABLE OldSchools RESTART IDENTITY;";

                command.ExecuteNonQuery();

                transaction.Commit();
            }
        }
    }
}
