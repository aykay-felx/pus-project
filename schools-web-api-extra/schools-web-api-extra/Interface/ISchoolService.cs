using schools_web_api_extra.Models;

namespace schools_web_api_extra.Interface;

public interface ISchoolService
{
    /// <summary>
    /// 1) Извлечь список школ из внешнего API, вернуть их как List<NewSchool>.
    /// </summary>
    Task<List<NewSchool>> FetchSchoolsFromApiAsync(int page);

    /// <summary>
    /// 2) Сравнить список NewSchool с уже существующими OldSchools 
    ///    (для каждого NewSchool заполнить SubFields, isDifferentObj, isNewObj).
    /// </summary>
    Task<List<NewSchool>> CompareWithOldSchoolsAsync(List<NewSchool> newSchools);

    /// <summary>
    /// 3) Сохранить список NewSchool в таблицу NewSchools (для последующего отображения на фронте).
    /// </summary>
    Task SaveNewSchoolsAsync(List<NewSchool> newSchools);

    /// <summary>
    /// 4) По запросу пользователя (после ручных корректировок) применить изменения 
    ///    из списка NewSchool к OldSchools:
    ///    - Если записи с таким RspoNumer нет, делаем INSERT.
    ///    - Если есть, делаем «частичное» обновление только по нужным полям.
    /// </summary>
    Task ApplyChangesFromNewSchoolsAsync(IEnumerable<NewSchool> newSchools);

    /// <summary>
    /// 5) Получить все старые школы (OldSchools).
    /// </summary>
    Task<IEnumerable<OldSchool>> GetAllOldSchoolsAsync();


    /// <summary>
    /// 5) Получить все старые школы (OldSchools).
    /// </summary>
    Task<IEnumerable<NewSchool>> GetAllNewSchoolAsync();

    /// <summary>
    /// 6) Удалить одну запись OldSchools по RspoNumer.
    /// </summary>
    Task DeleteOldSchoolAsync(string rspoNumer);
    Task<IEnumerable<SchoolHistory>> GetHistoryByRspoAsync(string rspoNumer);


    /// <summary>
    /// Новый метод: Сохранить изменения из списка NewSchool в OldSchools 
    /// (insert/update), а затем принудительно поменять поле Nazwa на '1' 
    /// для каждой затронутой школы.
    /// </summary>
    Task SaveOldSchoolFromApplyChangesAsync();
    Task DeleteAllNewSchoolAsync();

}