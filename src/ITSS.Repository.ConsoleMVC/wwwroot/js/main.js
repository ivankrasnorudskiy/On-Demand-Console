// main.js

const searchesList = $('.searches-list');
const searchInput = $('.search-input');
const createSearchButton = $('.button--create-search');
const getSearchResultsButton = $('.button--get-result');
const errorMessageContainer = $('.search-input__error-message');

const fragment = document.createDocumentFragment();
let intervalID = null;
let selectedListElement = null;
let selectedListElementId = null;
const INTERVAL_VALUE = 3000;
const BUTTON_NAME = {
    IN_PROGRESS: "Wait...",
    DEFAULT: "Create Search"
};
const LIST_ELEMENT_TEMPLATE =
    `<td class="element-number"></td>
    <td class="element-id"></td>
    <td class="element-status"></td>
    <td class="element-delete-button">X</td>`;

// при нажатии на Create Search Button создается новый поиск и обновляется общий список всех поисков

const onCreateSearchButtonClick = () => {
    if (searchInput.val()) {
        hideErrorMessage();
        createSearchButton[0].textContent = BUTTON_NAME.IN_PROGRESS;
        createSearchButton.attr('disabled', 'disabled');
        const search = {
            query: searchInput.val()
        };

        const json = JSON.stringify(search);

        $.ajax({
            type: 'POST',
            url: 'Searches',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: json
        }).done(function () {
            createSearchButton[0].textContent = BUTTON_NAME.DEFAULT;
            createSearchButton.removeAttr('disabled', 'disabled');
            searchInput.val('');
            updateSearchesList();
        });
    }
    else {
        showErrorMessage();
    }

};

// отображение сообщения об ошибке

const showErrorMessage = () => {
    searchInput[0].classList.add('search-input--invalid');
    errorMessageContainer[0].classList.remove('visually-hidden');
};

// скрытие сообщения об ошибке

const hideErrorMessage = () => {
    searchInput[0].classList.remove('search-input--invalid');
    errorMessageContainer[0].classList.add('visually-hidden');
};

// отображение списка поисков при открытии приложения

const displaySearchesList = () => {
    $.ajax({ url: 'Searches', type: 'get' }).done(function (searches) {
        if (searches.length > 0) {
            fillSearchesList(searches);
        }
        else {
            searchesList[0].textContent = 'Список пуст';
        }
    });
};


// для обновления списка всех поисков отправляется запрос на сервер, на основании полученных данных отображается обновленный список

const updateSearchesList = () => {
    $.ajax({ url: 'Searches', type: 'get' }).done(function (searches) {
        if (searches.length > 0) {
            fillSearchesList(searches);
            setUpdate(searches);
        }
    });
};

// проверка на необходимость обновления списка

const setUpdate = (searches) => {
    const needToUpdate = searches.some(element => element.state === "Running");
    if (needToUpdate && !intervalID) {
        console.log("неоходимо обновлять список");
        intervalID = setInterval(function () {
            updateSearchesList();
        }, INTERVAL_VALUE);

    } else if (!needToUpdate && intervalID) {
        clearInterval(intervalID);
        intervalID = null;
        console.log("все поиски завершены");
    }
};

// отрисовка списка поисков

const fillSearchesList = (searches) => {
    searchesList.empty();
   

    searches.forEach((search, index) => {
        const searchElementContainer = createSearchListElement(search, index);
        if (selectedListElement && search.id === selectedListElementId) {
            searchElementContainer.classList.add('searches-list__element--selected');
        }
        fragment.append(searchElementContainer);
    });
    searchesList[0].appendChild(fragment);
};

const createSearchListElement = (search, index) => {
    const searchElement = document.createElement('tr');
    searchElement.innerHTML = LIST_ELEMENT_TEMPLATE;
    searchElement.className = 'searches-list__element';
    searchElement.querySelector(".element-number").innerText = index;
    searchElement.querySelector(".element-id").innerText = search.id;
    searchElement.querySelector(".element-status").innerText = search.state;
    return searchElement;
};


// снятие выделение с элемента списка

const removeSelection = () => {
    $('.searches-list__element').each(function (index, value) {
        if (value.classList.contains('searches-list__element--selected')) {
            value.classList.remove('searches-list__element--selected');
        }
        selectedListElement = null;
        selectedListElementId = null;
    });
};

// выделение элемента по клику

const onSearchesListElementClick = ($event) => {
    const target = $event.target;
    if (target.parentNode.classList && target.parentNode.classList.contains('searches-list__element')) {
        removeSelection();
        target.parentNode.classList.add('searches-list__element--selected');
        selectedListElement = target.parentNode;
        selectedListElementId = selectedListElement.querySelector('.element-id').innerHTML;
    }
    else {
        removeSelection();
    }
};

// запрос результатов поиска

const onGetSearchResultButtonClick = () => {
    const searchId = selectedListElement.querySelector('.element-id').innerHTML;
    window.open('Searches/' + searchId + '/results');
};

const onDeleteButtonClick = ($event) => {
    const target = $event.target;
    if (target.classList.contains("element-delete-button")) {
        const searchId = target.parentNode.querySelector('.element-id').innerHTML; 
        $.ajax({ url: 'Searches/' + searchId, type: 'delete' }).done(function () {
            updateSearchesList();
        });
    }
};

displaySearchesList();

// слушатели событий

createSearchButton[0].addEventListener('click', onCreateSearchButtonClick);
window.addEventListener('click', onSearchesListElementClick);
getSearchResultsButton[0].addEventListener('click', onGetSearchResultButtonClick);
searchesList[0].addEventListener('click', onDeleteButtonClick);