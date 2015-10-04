chrome.browserAction.onClicked.addListener(() => {
	window.open("cloudmarks.html");

	/*chrome.bookmarks.create({
		parentId: "1",
		title: "Cloudmarks"
	}, (result: chrome.bookmarks.BookmarkTreeNode) => {
		console.log("done");
		console.log(result);
	});*/
});
