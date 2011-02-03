<?php
class AdminLogPager extends TablePager
{

	function getQueryInfo()
	{
		$queryInfo = array();
		
		
		$queryInfo['tables'] = 'adminlog';
		$queryInfo['fields'] = array('adminlog_id', 'adminlog_date','adminlog_user', 'adminlog_message');
		
		return $queryInfo;
	}

	function getFieldNames()
	{
		return array(
		'adminlog_id'	=> "Log ID",
		'adminlog_date' => "Date",
		'adminlog_user' => "Username",
		'adminlog_message' => "Message",
		);
	}

}