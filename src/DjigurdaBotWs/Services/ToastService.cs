﻿using System;

namespace DjigurdaBotWs.Services
{
    public interface IToastService
    {
        string GetRandomToast();
    }

    public class ToastService : IToastService
    {
        private readonly Random _random;
        private readonly string[] _toasts;

        public ToastService()
        {
            _random = new Random();
            _toasts = new[]
            {
                "Когда падает звезда, говорят, что это к счастью. Я хочу выпить за то, чтобы в твоей жизни был звездопад.",
                "Пусть тебе всегда светят звезды - звезда удачи, звезда любви,звезды дружбы и звезды коньяка.",
                "Пусть твоя жизнь будет безоблачной, как небо над пустыней, полной, как кувшин хлебосольного кавказца, долгой и интересной, как Млечный Путь. И пусть всегда светятся счастьем твои глаза, как полярное сияние над полюсом!",
                "В день рождения не человек становится на один год старше - это еще один год пополняется таким прекрасным человеком, как ты!",
                "Не стоит огорчаться, что ты стала на один год старше. Если взглянуть с другой стороны, ты стала на один год красивее, привлекательнее и умнее! А на этом пути нельзя останавливаться. Поздравляю тебя!",
                "Пусть будет все, как хочешь ты. Пусть ожиданья не обманут. А все прекрасные мечты. Твоей действительностью станут!",
                "За тебя бокал я поднимаю! За твою мудрость, красоту. Счастья женского тебе желаю. И желаю воплотить мечту!",
                "Говорят, что критика легка, творчество трудно. Так выпьем же за виновника торжества, настоящего творца, никого не критикующего!",
                "Один из восточных мудрецов сказал: \"Легко сделать так, чтобы тебя знали люди, но трудно воистину познать самого себя\". Я хочу пожелать имениннику, чтобы он познал себя. Это поможет ему достичь благородной цели, которую он перед собой поставил. С днем рождения!",
                "Давайте поднимем бокалы и выпьем за то, что бы твоя жизнь была так же длинна, как и река что пропадает за горизонтом, что бы в твоем доме всегда было уютно, а друзья и близкие всегда были рядом. С днем рожденья и побольше счастья в твоей реке!"
            };
        }

        public string GetRandomToast()
        {
            var randomIndex = _random.Next(0, _toasts.Length - 1);
            return _toasts[randomIndex];
        }
    }
}
