//https://threejsfundamentals.org/threejs/lessons/threejs-picking.html

(function () {
    //#region  Shared variables
    let editor;
    let nodes = new Array(), grids;
    let columns = new Array(), mainBeams = new Array(), secondaryBeams = new Array(), sections = new Array();
    let canvas, domEvents;
    let levels;
    let draw = false, drawingPoints = [];
    let sectionId = 0;
    //#endregion
    function retrocycle(o) {
        var self = this;
        self.identifiers = [];
        self.refs = [];

        self.rez = function (value) {

            // The rez function walks recursively through the object looking for $ref
            // properties. When it finds one that has a value that is a path, then it
            // replaces the $ref object with a reference to the value that is found by
            // the path.

            var i, item, name, path;

            if (value && typeof value === 'object') {
                if (Object.prototype.toString.apply(value) === '[object Array]') {
                    for (i = 0; i < value.length; i += 1) {
                        item = value[i];
                        if (item && typeof item === 'object') {
                            path = item.$ref;
                            if (typeof path === 'string' && path != null) {
                                //self.refs[parseInt(path)] = {};

                                value[i] = self.identifiers[parseInt(path)]
                            } else {
                                self.identifiers[parseInt(item.$id)] = item;
                                self.rez(item);
                            }
                        }
                    }
                } else {
                    for (name in value) {
                        if (typeof value[name] === 'object') {
                            item = value[name];
                            if (item) {
                                path = item.$ref;
                                if (typeof path === 'string' && path != null) {
                                    value[name] = self.identifiers[parseInt(path)]
                                } else {
                                    self.identifiers[parseInt(item.$id)] = item;
                                    self.rez(item);
                                }
                            }
                        }
                    }
                }
            }

        };
        self.rez(o);
        self.identifiers = [];
    }
    function init() {
        editor = new Editor(); //Instantiate editor
        debugger
        canvas = editor.renderer.domElement;        
        return $.ajax({
            url: '/Outputs/Saved/Model.json',
            success: function (data) {
                debugger
                retrocycle(data);
                buildModel(data);
            },
            error: function (x, y, err) {
                debugger
                console.log(err)
            }
        });

        //$('#exampleModal').modal('show'); //Temporary data input
    }

    function buildModel(model) {
        editor.init(model.grids.coordX[model.grids.coordX.length - 1], model.grids.coordZ[model.grids.coordZ.length - 1]); //Setup editor

        grids = new Grid(model.grids.coordX, model.grids.coordZ, 4.5, model.grids.levels);

        levels = grids.levels;
        editor.addToGroup(grids.gridLines, 'grids'); //Add x-grids to scene (as a group)this.meshInX
        editor.addToGroup(grids.gridNames, 'grids'); //Add z-grids to scene (as a group)
        editor.addToGroup(grids.axes, 'grids'); //Add z-grids to scene (as a group)
        editor.addToGroup(grids.dimensions, 'dimensions');

        sections = model.sections;
        sectionId = parseInt(model.sections[model.sections.length - 1].$id);
        generateNodes(model.nodes, nodes);

        secondaryBeams.push(Beam.generate(model.secondaryBeams, editor));
        mainBeams.push(Beam.generate(model.mainBeams, editor));
        columns.push(Column.generate(model.columns, editor));     
    }

    function generateNodes(modelNodes, nodes) {        
        for (let i = 0; i < modelNodes.length; i++) {
            let node = Node.create(modelNodes[i].position.x, modelNodes[i].position.y, modelNodes[i].position.z, modelNodes[i].support, editor, nodes, modelNodes[i].$id);
            for (let j = 0; j < modelNodes[i].pointLoads.length; j++) {
                node.addPointLoad(new PointLoad(modelNodes[i].pointLoads[j].magnitude, modelNodes[i].pointLoads[j].pattern))
            }
            modelNodes[i].data = node.data; //switch modelNodes position to Vector3
        }
    }

    init();

    canvas.addEventListener('mousemove', function (event) {
        editor.pick(event);
    });

    //Try Area selection
    let initialPosition;
    let multiple = false;
    canvas.onmousedown = function (event) {
        initialPosition = editor.setPickPosition(event);
    }

    let finalPosition;
    canvas.onmouseup = function (event) {
        finalPosition = editor.setPickPosition(event);
        if (initialPosition.x === finalPosition.x && initialPosition.y === finalPosition.y) {
            editor.select(event, multiple);
            if (draw) {
                if (editor.picker.selectedObject.size == 1) {
                    for (let item of editor.picker.selectedObject) {
                        if (item.userData.node) {
                            drawingPoints.push(item.userData.node);
                            drawingPoints[0].visual.mesh.material.color.setHex(0xcc0000); //Highlight the first node
                        }
                    }
                    if (drawingPoints.length === 2) {
                        drawElement();
                    }
                }
            }
        }
        else {
            let rectWidth = Math.abs(finalPosition.x - initialPosition.x),
                rectHeight = Math.abs(finalPosition.y - initialPosition.y);

            //The start position of the rectangle sholud be the top left corner
            if (finalPosition.x < initialPosition.x)
                initialPosition.x = finalPosition.x;
            if (finalPosition.y < initialPosition.y)
                initialPosition.y = finalPosition.y;
            editor.selectByArea(initialPosition, rectWidth, rectHeight, multiple);
        }
    }


    window.addEventListener('keyup', function (event) {
        switch (event.key) {
            case 'Delete':
                deleteElement();
                break;

            case 'm':
                move();
                break;

            case 'c':
                copy();
                break;

            case 'd':
                draw = draw ? false : true;
                break;

            case 'Control':
                multiple = false;
                break;
        }
    });

    function drawElement() {
        let element;
        let sectionName = $('#drawSection').val();
        let sectionObject = sections.find(s => s.name === sectionName); //Check if section already exists
        if (!sectionObject) { //If not existing , create one
            sectionObject = { $id: `${sectionId += 1000}`, name: sectionName };
            sections.push(sectionObject);
        }
        let start = drawingPoints[0], end = drawingPoints[1];
        if (drawingPoints[1].data.position.y < drawingPoints[0].data.position.y) {
            start = drawingPoints[1];
            end = drawingPoints[0];
        }
        let index = levels.indexOf(end.data.position.y) - 1;
        if (index < 0) {
            drawingPoints = [];
            alert('please use one of the predefined levels');
            return;
        }
        else {
            let beam = editor.getIntersected(start.data.position.clone(), start.visual.mesh.userData.picking);
            if (beam) {
                Beam.switchType(beam.userData.element, secondaryBeams[index], mainBeams[index]);
            }
            beam = editor.getIntersected(end.data.position.clone(), end.visual.mesh.userData.picking);
            if (beam) {
                Beam.switchType(beam.userData.element, secondaryBeams[index], mainBeams[index]);
            }

            if (start.data.position.x == end.data.position.x &&
                start.data.position.z == end.data.position.z) { //Check if the element is vertical(column)
                element = drawColumnByTwoPoints(sectionObject, start, end);
                columns[index].push(element);
            }
            else {//Element is not vertical (Beam)
                element = drawBeamByTwoPoints(sectionObject, drawingPoints[0], drawingPoints[1]);
                secondaryBeams[index].push(element);
            }
            editor.addToGroup(element.visual.mesh, 'elements');
            editor.createPickingObject(element);
            drawingPoints[0].visual.mesh.material.color.setHex(0xffcc00); //Restore the first node color
            editor.picker.unselect(); // Unselect the second node
            drawingPoints = [];
        }
    }


    window.onkeydown = (event) => {
        if (event.key === 'Control')
            multiple = true;
    }

    window.deleteElement = function () {
        for (let item of editor.picker.selectedObject) {
            if (item.userData.element instanceof Beam) {
                editor.removeFromGroup(item, 'elements');
                let found = false;
                for (var i = 0; i < secondaryBeams.length; i++) {
                    index = secondaryBeams[i].indexOf(item.userData.element);
                    if (index > -1) {
                        secondaryBeams[i].splice(index, 1);
                        found = true;
                        break;
                    }
                }
                if (!found) {
                    for (var i = 0; i < mainBeams.length; i++) {
                        let index = mainBeams[i].indexOf(item.userData.element);
                        if (index > -1) {
                            mainBeams[i].splice(index, 1);
                            break;
                        }
                    }
                }
            }
            else if (item.userData.element instanceof Column) {
                editor.removeFromGroup(item, 'elements');
                for (var i = 0; i < columns.length; i++) {
                    let index = columns[i].indexOf(item.userData.element);
                    if (index > -1) {
                        columns[i].splice(index, 1);
                        break;
                    }
                }
            }
            else {
                editor.removeFromGroup(item, 'nodes');
                let index = nodes.indexOf(item.userData.node)
                nodes.splice(index, 1);
            }
        }
        editor.picker.selectedObject.clear();
    }

    window.move = function () {
        let displacement = new THREE.Vector3(parseFloat($('#xMove').val()) || 0, parseFloat($('#yMove').val()) || 0, parseFloat($('#zMove').val()) || 0)
        for (let item of editor.picker.selectedObject) {
            if (item.userData.element) {//Beams or Cloumns only
                item.userData.element.move(displacement);
                let newStartPosition = item.position;
                let newEndPosition = item.userData.element.visual.endPoint;
                let levelIndex = levels.indexOf(newEndPosition.y) - 1;
                if (levelIndex > -1) {
                    //Check if nodes already exist at the new position or create new ones.
                    getElementNodes(newStartPosition, newEndPosition, item.userData.element, levelIndex);
                    item.userData.picking.position.copy(newStartPosition);
                }
                else {
                    item.userData.element.move(displacement.multiplyScalar(-1));
                    alert('please move elements to one of the predefined levels');
                }
            }
        }
    }

    window.copy = function () {
        let displacement = new THREE.Vector3(parseFloat($('#xCopy').val()) || 0, parseFloat($('#yCopy').val()) || 0, parseFloat($('#zCopy').val()) || 0)
        let replication = parseInt($('#Replication').val());
        for (let item of editor.picker.selectedObject) {
            if (item.userData.element) { //Beams or Cloumns only
                let element = item.userData.element;
                for (var i = 0; i < replication; i++) {
                    element = element.clone();
                    element.move(displacement);

                    let levelIndex = levels.indexOf(element.visual.endPoint.y) - 1;

                    if (levelIndex > -1) {
                        //Check if nodes already exist at the new position or create new ones.
                        getElementNodes(element.visual.mesh.position, element.visual.endPoint, element, levelIndex);


                        if (element instanceof Beam)
                            secondaryBeams[levelIndex].push(element);
                        else
                            columns[levelIndex].push(element);

                        editor.addToGroup(element.visual.mesh, 'elements');
                        editor.createPickingObject(element);
                    }
                    else {
                        element.move(displacement.multiplyScalar(-1));
                        alert('please move elements to one of the predefined levels');
                    }
                }
            }
        }
    }

    function getElementNodes(newStartPosition, newEndPosition, element, levelIndex) {
        //Search for the new nodes in the existing nodes
        let newStartNode = nodes.find(n => n.data.position.equals(newStartPosition));
        let newEndNode = nodes.find(n => n.data.position.equals(newEndPosition));

        if (!newStartNode) { //If it doesn't exist create one
            newStartNode = Node.create(newStartPosition.x, newStartPosition.y, newStartPosition.z,
                null, editor, nodes);
        }

        let beam = editor.getIntersected(newStartNode.data.position.clone());
        if (beam) {
            Beam.switchType(beam.userData.element, secondaryBeams[levelIndex], mainBeams[levelIndex]);
            beam.userData.element.data.innerNodes.push({ '$ref': newStartNode.data.$id });
        }

        if (!newEndNode) {//If it doesn't exist create one
            newEndNode = Node.create(newEndPosition.x, newEndPosition.y, newEndPosition.z, null, editor, nodes);
        }
        beam = editor.getIntersected(newEndNode.data.position.clone());
        if (beam) {
            Beam.switchType(beam.userData.element, secondaryBeams[levelIndex], mainBeams[levelIndex]);
            beam.userData.element.data.innerNodes.push({ '$ref': newEndNode.data.$id });
        }

        element.data.startNode = { "$ref": newStartNode.data.$id };
        element.data.endNode = { "$ref": newEndNode.data.$id };
    }

    window.toggle = function () {
        editor.toggleBeams();
    }

    window.addFloorLoad = function () {
        let load = new LineLoad($('#floorLoadCase').val(), this.parseFloat($('#floorLoad').val()));
        editor.clearGroup('loads');
        for (let i = 0; i < secondaryBeams.length; i++) { //(for) is faster than (forEach)
            let index = secondaryBeams[i].addLoad(load, true);
            editor.addToGroup(secondaryBeams[i].data.loads[index].render(secondaryBeams[i]), 'loads');
        }
    }

    window.addLineLoad = function () { //Adds a LineLoad to the selected beam
        editor.clearGroup('loads');
        let replace = $('#replaceLineLoad').prop('checked'); //Wether to replace the existing load (if any) or add to it
        let load = new LineLoad(parseFloat($('#lineLoad').val()), $('#lineLoadCase').val());
        debugger
        for (let element of editor.picker.selectedObject) {
            if (element.userData.element instanceof Beam) {
                let beam = element.userData.element;
                let loadIndex = beam.addLoad(load, replace);
                editor.addToGroup(beam.data.lineLoads[loadIndex].render(beam), 'loads')
            }
        }
    }


    window.addPointLoad = function () { //Adds a PointLoad to the selected node
        editor.clearGroup('loads');
        let replace = $('#replacePointLoad').prop('checked'); //Wether to replace the existing load (if any) or add to it
        let pointLoad = new PointLoad(parseFloat($('#pointLoad').val()), $('#pointLoadCase').val());
        for (let element of editor.picker.selectedObject) {
            if (element.userData.node) {
                let node = element.userData.node;
                let loadIndex = node.addLoad(pointLoad, replace);
                editor.addToGroup(node.data.pointLoads[loadIndex].render(node.data.position.clone()), 'loads')
            }
        }
    }

    window.hideLoads = function () {
        editor.clearGroup('loads');
    }

    window.showLoads = function () { // Visualize all load in the selected case
        let pattern = $('#showLoadCase').val();
        editor.clearGroup('loads');

        let index;
        for (let i = 0; i < secondaryBeams.length; i++) {
            for (let j = 0; j < secondaryBeams[i].length; j++) {
                index = secondaryBeams[i][j].data.lineLoads.findIndex(l => l.pattern == pattern);
                if (index > -1)
                    editor.addToGroup((secondaryBeams[i][j].data.lineLoads[index]).render(secondaryBeams[i][j]), 'loads');
            }
        }

        for (let i = 0; i < mainBeams.length; i++) {
            for (let j = 0; j < mainBeams[i].length; j++) {
                index = mainBeams[i][j].data.lineLoads.findIndex(l => l.pattern == pattern);
                if (index > -1)
                    editor.addToGroup((mainBeams[i][j].data.lineLoads[index]).render(mainBeams[i][j]), 'loads');
            }
        }


        for (let i = 0; i < nodes.length; i++) {
            index = nodes[i].data.pointLoads.findIndex(l => l.pattern == pattern);
            if (index > -1)
                editor.addToGroup((nodes[i].data.pointLoads[index]).render(nodes[i].data.position.clone()), 'loads');
        }
    }

    window.changeSection = function () {
        let sectionName = $('#section').val();
        let existingSection = sections.find(s => s.name == sectionName);//Check if the section already exists
        if (!existingSection) {//if not create a new one
            existingSection = { $id: `${sectionId += 1000}`, name: sectionName };
            sections.push(existingSection);
        }
        for (let item of editor.picker.selectedObject) {
            if (item.userData.element) { // Beams and columns only
                item.userData.element.changeSection(existingSection);
            }
        }
    }

    window.addNodeToBeam = function () {
        let distances = $('#nodeToBeam').val().split(',').map(d => this.parseFloat(d));
        let element, createdNode;
        for (let item of editor.picker.selectedObject) {
            if (item.userData.element instanceof Beam) {
                element = item.userData.element;
                for (var i = 0; i < distances.length; i++) {
                    let displacement = element.visual.direction.clone().multiplyScalar(distances[i]);
                    let nodePosition = item.position.clone().add(displacement);

                    createdNode = Node.create(nodePosition.x, nodePosition.y, nodePosition.z,
                        null, editor, nodes);
                    element.data.innerNodes.push({ $ref: createdNode.data.$id });
                }
            }
        }
    }

    window.createNode = function () {
        let node = Node.create(parseFloat($('#nodeXCoord').val()),
            parseFloat($('#nodeYCoord').val()), parseFloat($('#nodeZCoord').val()), null, editor, nodes);

        let beam = editor.getIntersected(node.data.position.clone()); //Beam mesh
        if (beam && beam.userData.element instanceof Beam) {
            beam = beam.userData.element;
            beam.data.innerNodes.push({ "$ref": node.data.$id }); //Add the node to the beam inner nodes
        }
    }

    window.startDrawMode = () => draw = true;
    window.endDrawMode = () => {
        draw = false;
        if (drawingPoints[0])
            drawingPoints[0].visual.mesh.material.color.setHex(0xffcc00); //Restore the first node color

        drawingPoints = [];
    }

    function createModel() { //Serialize model components to JSON
        let model = {
            nodes: [], material: { '$id': 'm', name: 'ST_37' }, sections: [],
            secondaryBeams: [], mainBeams: [], columns: [], grids: {}
        };

        model.projectProperties = {
            "Number": "1",
            "Name": "AUTRA2",
            "Designer": "AUTRA2",
            "Location": "Smart Village",
            "City": "Giza",
            "Country": "Egypt",
            "Owner": "AUTRA"
        }
        for (var i = 0; i < nodes.length; i++) {
            model.nodes.push(nodes[i].data);
        }

        model.sections = sections;

        for (var i = 0; i < sections.length; i++) {
            sections[i].material = { $ref: "m" };
        }

        for (var i = 0; i < secondaryBeams[0].length; i++) {
            model.secondaryBeams.push(secondaryBeams[0][i].data);
        }

        for (var i = 0; i < mainBeams[0].length; i++) {
            model.mainBeams.push(mainBeams[0][i].data);
        }

        for (var i = 0; i < columns[0].length; i++) {
            model.columns.push(columns[0][i].data);
        }
        model.grids.coordX = grids.coordX;
        model.grids.coordZ = grids.coordZ;
        model.grids.levels = grids.levels;
        model = JSON.stringify(model);
        console.log(model)
        return model;
    }

    window.solve = function () { //Send data to server        
        editor.clearGroup('loads');
        $.ajax({
            url: `/Editor/Solve`,
            type: "POST",
            contentType: 'application/json',
            data: createModel(),
            success: function (res) {
                debugger
                if (domEvents)
                    domEvents.destroy();//Clear old events (if existing)
                domEvents = new THREEx.DomEvents(editor.camera, canvas);

                res = JSON.parse(res);
                editor.clearGroup('results');
                editor.hideGroup('nodes');
                for (let i = 0; i < mainBeams[0].length; i++) {
                    mainBeams[0][i].visual.strainingActions = res.mainBeams[i].strainingActions;
                    mainBeams[0][i].visual.strainingActions.push(res.mainBeams[i].combinedSA[0]);
                    editor.addToGroup(mainBeams[0][i].showMoment('dead', 0, domEvents), 'results');
                }

                for (let i = 0; i < secondaryBeams[0].length; i++) {
                    secondaryBeams[0][i].visual.strainingActions = res.secondaryBeams[i].strainingActions;
                    secondaryBeams[0][i].visual.strainingActions.push(res.secondaryBeams[i].combinedSA[0]);
                    editor.addToGroup(secondaryBeams[0][i].showMoment('dead', 0, domEvents), 'results');
                }

                for (let i = 0; i < columns[0].length; i++) {
                    columns[0][i].visual.strainingActions = res.columns[i].strainingActions;
                    columns[0][i].visual.strainingActions.push(res.columns[i].combinedSA[0]);
                }

                for (let i = 0; i < columns[0].length; i++) {
                    nodes[i].visual.reactions = res.supports[i].reactions;
                }
            },
            error: function (x, y, res) {
                console.log(res)
            }
        });
    }

    window.save = function () { // Save data on the server
        $.ajax({
            url: `/Editor/Save`,
            type: "POST",
            contentType: 'text/plain',
            data: createModel(),
            success: function (res) {
                console.log(res)
            },
            error: function (x, y, res) {
                console.log(res)
            }
        });
    }

    window.downloadFile = function () {

        this.localStorage.setItem('Model', createModel()); //Save data to localStorage ??!! Option #1

        //Save data on client machine if no internet connection Option #2
        let text = new Blob([createModel()], { type: 'text/json' }); //Blob : An object that represents a file

        let textFile = window.URL.createObjectURL(text); // The URL to that object

        let link = document.createElement('a'); //Create HTML link to download the file on client machine
        link.setAttribute('download', 'info.json');
        link.href = textFile;
        document.body.appendChild(link);

        this.setTimeout(function () { // domElement takes some time to be added to the document
            link.click(); //Fire the click event of the link
            document.body.removeChild(link); //The link is no longer needed
            URL.revokeObjectURL(textFile); // Dispose the URL Object
        }, 1000);
    }

    /*$('#upload').change(function (event) { //Read data from uploaded file
        debugger
        let file = event.target.files[0];
        var reader = new FileReader();
        reader.onload = function (evt) {
            let obj = JSON.parse(evt.target.result);
            console.log(obj);
        };
        reader.readAsText(file);
    });*/

    //used to toggle between dark and light themes
    window.darkTheme = () => editor.darkTheme();

    window.lightTheme = () => editor.lightTheme();

    window.screenshot = () => editor.screenshot();

    window.hideDimensions = () => {
        editor.hideGroup('dimensions');
        $('#hideDimensions').css('display', 'none');
        $('#showDimensions').css('display', 'block');
    }

    window.showDimensions = () => {
        editor.showGroup('dimensions');
        $('#showDimensions').css('display', 'none');
        $('#hideDimensions').css('display', 'block');
    }
    window.result = () => {
        editor.clearGroup('results');
        editor.hideGroup('nodes'); //Temporarily hide nodes (for clearer display of stations)
        let pattern = $('#resultPattern').val();
        let strainingAction = $('#strainingAction').val();
        let display = parseInt($('#display').val());
        debugger
        if (domEvents)
            domEvents.destroy();//Clear old events (if existing)
        domEvents = new THREEx.DomEvents(editor.camera, canvas);
        switch (strainingAction) {
            case 'Mo':
                for (var i = 0; i < mainBeams.length; i++) {
                    for (var j = 0; j < mainBeams[i].length; j++) {
                        editor.addToGroup(mainBeams[i][j].showMoment(pattern, display, domEvents), 'results');
                    }
                }
                for (var i = 0; i < secondaryBeams.length; i++) {
                    for (var j = 0; j < secondaryBeams[i].length; j++) {
                        editor.addToGroup(secondaryBeams[i][j].showMoment(pattern, display, domEvents), 'results');
                    }
                }
                break;
            case 'V':
                for (var i = 0; i < secondaryBeams.length; i++) {
                    for (var j = 0; j < secondaryBeams[i].length; j++) {
                        editor.addToGroup(secondaryBeams[i][j].showShear(pattern, display, domEvents), 'results');
                    }
                }
                for (var i = 0; i < mainBeams.length; i++) {
                    for (var j = 0; j < mainBeams[i].length; j++) {
                        editor.addToGroup(mainBeams[i][j].showShear(pattern, display, domEvents), 'results');
                    }
                }
                break;
            case 'No':
                for (var i = 0; i < columns.length; i++) {
                    for (var j = 0; j < columns[i].length; j++) {
                        editor.addToGroup(columns[i][j].showNormal(pattern, display, domEvents), 'results');
                    }
                }
                break;
            case 'rv':
                for (var i = 0; i < nodes.length; i++) {
                    if (nodes[i].data.support)
                        editor.addToGroup(nodes[i].showReaction(pattern), 'results');
                    else
                        break;
                }
                break;
        }
    };

    window.hideResults = () => {
        if (domEvents)
            domEvents.destroy();//Clear old events (if existing)

        editor.clearGroup('results'); //Clear diplayed results
        editor.showGroup('nodes'); //Display nodes again
    }
})();