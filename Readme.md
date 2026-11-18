Что реализовано:
/catalog/categories эндпоинт на получение списка категорий
/catalog/objects эндпоинт на получение всех объектов, доступные переменные page страница, category_id фильтрация по конкретной категории и q поисковый запрос
/catalog/objects/{id} эндпоинт на получение конкретного объекта по id

Кроме этого есть 4 эндпоинта для начального заполнения категорий объектами при помощи внешних API
/catalog/objects/films/import получаем фильмы при помощи unofficial kinopoisk api
/catalog/objects/anime/import получаем аниме при помощи shikimori api
/catalog/objects/games/import получаем игры при помощи steam api
/catalog/objects/books/import получаем книги при помощи ReBooks api
