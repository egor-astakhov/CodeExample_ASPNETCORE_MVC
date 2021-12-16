$(function () {
    initEvents();

    function initEvents() {

        initDynamicRows();

        function initDynamicRows() {
            const CONTAINER = ".dynamic-row-container";
            const ROW = ".dynamic-row";
            const FIRST_ROW = ".dynamic-row:first";
            const ADD_BUTTON = ".dynamic-row-add";
            const DELETE_BUTTON = ".dynamic-row-delete";

            $(ADD_BUTTON).each(function eachAddButton() {
                let containerId = $(this).data("target");
                let $container = $("#" + containerId);

                let $templateRow = $container.find(FIRST_ROW).clone(false);
                clearRow($templateRow);
                $templateRow.find(DELETE_BUTTON).click(deleteDynamicRow);

                $(this).click(function addDynamicRow() {
                    let $clonedRow = $templateRow.clone(true);
                    $container.append($clonedRow);

                    toggleDeleteButtonsVisibility($container);
                    updateIndexes($container);

                    $clonedRow.find(".datepicker").pickadate({
                        format: Integral.dateFormat
                    });
                });
            });

            $(DELETE_BUTTON).click(deleteDynamicRow);

            function deleteDynamicRow() {
                let $row = $(this).closest(ROW);
                let $container = $row.closest(CONTAINER);

                let isSoleRow = $container.find(ROW).length === 1;
                if (isSoleRow && !allowEmptyContainer($container)) {
                    return;
                }

                $row.remove();

                toggleDeleteButtonsVisibility($container);
                updateIndexes($container);
            }

            function toggleDeleteButtonsVisibility($container) {
                if (allowEmptyContainer($container)) return;

                let $deleteButtons = $container.find(DELETE_BUTTON);
                if ($container.find(ROW).length > 1) {
                    $deleteButtons.show();
                } else {
                    $deleteButtons.hide();
                }
            }

            function updateIndexes($container) {
                $container.find(ROW).each(function (i) {
                    $row = $(this);
                    $row.find("input").each(function eachClonedInput() {
                        let id = $(this).attr("id");
                        let name = $(this).attr("name");

                        let idMatch = /(.+_)([\d]+)(__.+)/.exec(id);
                        if (idMatch) {
                            idMatch[2] = i;
                            let newId = idMatch.slice(1).join("");
                            $(this).attr("id", newId);
                            $row.find("label[for='" + id + "']").attr("for", newId);
                        }

                        let nameMatch = /(.+\[)(\d+)(\].+)/.exec(name);
                        if (nameMatch) {
                            nameMatch[2] = i;
                            let newName = nameMatch.slice(1).join("");
                            $(this).attr("name", newName);
                            $row.find("span[data-valmsg-for='" + name + "']").attr("data-valmsg-for", newName);
                        }
                    });
                });

                let form = $container.closest("form")
                    .removeData("validator")
                    .removeData("unobtrusiveValidation");

                $.validator.unobtrusive.parse(form);
            }

            function clearRow($row) {
                $row.find("input").each(function eachInput() {
                    if ($(this).attr("name").endsWith(".Id")) {
                        $(this).val("0");
                    } else {
                        $(this).val("");
                    }
                });
                $row.find(".field-validation-error").empty();
                $row.find("label").removeClass("active");
                return $row;
            }

            function allowEmptyContainer($container) {
                let allow = $container.data("allow-empty");
                if (allow === false) {
                    return false;
                }
                return true;
            }

            (function onDocumentReady() {
                $(CONTAINER).each(function eachContainer() {
                    let $container = $(this);
                    if (allowEmptyContainer($container)) {
                        deleteEmptyRow($container);
                    } else {
                        toggleDeleteButtonsVisibility($container);
                    }
                });
            })();

            function deleteEmptyRow($container) {
                let $row = $container.find(ROW);
                if ($row.length > 1) return;

                let isRowEmpty = $row
                    .find("input")
                    .map(function () {
                        return $(this).val();
                    })
                    .get()
                    .every(e => !e);

                if (isRowEmpty) {
                    $row.find(DELETE_BUTTON).click();
                }
            }
        }
    }
});
