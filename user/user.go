package user

import (
	"fmt"
	"log"
	"time"
)

type ToDo_List struct {
	Name   string
	Text   string
	Date   string
	IsDone bool
}

var ToDo = make(map[string][]ToDo_List)

func NewToDo() ToDo_List {
	log.Print("Добавление новой задачи")
	var name string
	var text string
	var date string
	var isdone bool
	newName(name)
	newText(text)
	newDate(date)
	newIsDone(isdone)
	newToDo := ToDo_List{
		Name:   name,
		Text:   text,
		Date:   date,
		IsDone: isdone,
	}
	defer func() {
		ToDo[name] = append(ToDo[name], newToDo)
	}()
	return newToDo
}

func newName(name string) {
	fmt.Println("Введите название задачи:")
	fmt.Scan(&name)
}
func newText(text string) {
	fmt.Println("Введите содержимое задачи задачи:")
	fmt.Scan(&text)
}
func newDate(date string) {
	date = time.Now().Format("2006-01-02 15:04")
}
func newIsDone(isdone bool) {
	isdone = false
}
