using UnityEngine;

public class UnitRagdollSpawner : MonoBehaviour, ISetupForSpawn // Юнит Тряпичная кукла Зарождение// Висит на юните
{
    [SerializeField] private Transform _ragdollPrefab; // Префаб тряпичной куклы
    [SerializeField] private Transform _originalRootBone; // Оригинальная Корневая Кость(юнита) // В инспекторе закинуть кость юнита под названием Root 

    private HealthSystem _healthSystem;
    private UnitActionSystem _unitActionSystem;

    private Unit _keelerUnit; //Сохраним юнита который хочет нас убить Киллер.
    private Unit _unit; // Юнит на котором лежит этот скрипт

    public void SetupForSpawn(Unit unit)
    {
        _unit = unit;
        _healthSystem = _unit.GetHealthSystem();
        _unitActionSystem = _unit.GetUnitActionSystem();

        _healthSystem.OnDead += HealthSystem_OnDead; // подпишемся на событие Умер (запустим событие когда юнит умер)
    }
       

    private void Start()
    {
        _healthSystem.OnDead += HealthSystem_OnDead; // подпишемся на событие Умер (запустим событие когда юнит умер)

        /*ShootAction.OnAnyShoot += ShootAction_OnAnyShoot; // Подпишемся на событие Любой Начал стрелять (на OnShoot не могу подписаться т.к. он не static и нужна ссылка на игровой объект)
        SwordAction.OnAnySwordHit += SwordAction_OnAnySwordHit; // Подпишемся на cобытие - Любой Начал удар мечом*/
    }

   /* private void SwordAction_OnAnySwordHit(object sender, SwordAction.OnSwordEventArgs e)
    {
        if (e.targetUnit == _unit) // Если целью является этот юнит , то сохраним того кто по нам стрелял
        {
            _keelerUnit = e.hittingUnit;
        }
    }

    private void ShootAction_OnAnyShoot(object sender, ShootAction.OnShootEventArgs e)
    {
        if (e.targetUnit == _unit) // Если целью является этот юнит , то сохраним того кто по нам стрелял
        {
            _keelerUnit = e.shootingUnit;
        }
    }*/


    private void HealthSystem_OnDead(object sender, System.EventArgs e)
    {
        Unit keelerUnit = _unitActionSystem.GetSelectedUnit();// Килер это активный юнит
        Transform ragdollTransform = Instantiate(_ragdollPrefab, transform.position, transform.rotation); // Создадим куклу из префаба в позиции юнита
        UnitRagdoll unitRagdoll = ragdollTransform.GetComponent<UnitRagdoll>(); // Найдем компонент UnitRagdoll на префабе

        unitRagdoll.Setup(_unitActionSystem, _originalRootBone, keelerUnit); // и передадим в метод Init трансформ Оригинальнай Корневой Кости и СТРЕЛКА
    }
}
